using System.Text.Json;
using Fortuna.Domain.Accounts.Events;
using Fortuna.Domain.Products.Events;
using Fortuna.Domain.Transfers.Events;
using Fortuna.Infrastructure.Persistence.Write;
using Fortuna.Infrastructure.Projections.Interfaces;
using Fortuna.ReadModel.Dashboard.ReadModels;
using Fortuna.ReadModel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fortuna.ReadModel.Projections;

public sealed class ProjectionDispatcher : IReadModelProjector
{
    private readonly ReadDbContext _dbContext;

    public ProjectionDispatcher(ReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ProjectAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        var alreadyProcessed = await _dbContext.ProcessedOutboxMessages
            .AsNoTracking()
            .AnyAsync(x => x.OutboxMessageId == message.Id, cancellationToken);

        if (alreadyProcessed)
        {
            return;
        }

        if (IsEventType<ProductCreatedDomainEvent>(message.Type))
        {
            await ProjectProductCreatedAsync(message, cancellationToken);
        }
        else if (IsEventType<MoneyDepositedDomainEvent>(message.Type))
        {
            await ProjectMoneyDepositedAsync(message, cancellationToken);
        }
        else if (IsEventType<MoneyWithdrawnDomainEvent>(message.Type))
        {
            await ProjectMoneyWithdrawnAsync(message, cancellationToken);
        }

        _dbContext.ProcessedOutboxMessages.Add(new ProcessedOutboxMessageReadModel
        {
            OutboxMessageId = message.Id,
            ProcessedAtUtc = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ProjectProductCreatedAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        var domainEvent = Deserialize<ProductCreatedDomainEvent>(message.Payload);

        var productTile = await _dbContext.ProductTiles.FindAsync([domainEvent.ProductId], cancellationToken);
        if (productTile is null)
        {
            productTile = new ProductTileReadModel
            {
                ProductId = domainEvent.ProductId
            };

            _dbContext.ProductTiles.Add(productTile);
        }

        productTile.CustomerId = domainEvent.CustomerId;
        productTile.ProductCategory = domainEvent.ProductCategory;
        productTile.ProductType = domainEvent.ProductType;
        productTile.ProductName = domainEvent.ProductName;
        productTile.ProductNumber = domainEvent.ProductNumber;
        productTile.Balance = domainEvent.Balance;
        productTile.Currency = domainEvent.Currency;
    }

    private async Task ProjectMoneyDepositedAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        var domainEvent = Deserialize<MoneyDepositedDomainEvent>(message.Payload);
        var productTile = await GetRequiredProductTileAsync(domainEvent.AccountId, cancellationToken);

        productTile.Balance += domainEvent.Amount;

        _dbContext.TimelineEvents.Add(new TimelineEventReadModel
        {
            Id = message.Id,
            CustomerId = domainEvent.CustomerId,
            ProductId = domainEvent.AccountId,
            EventDateUtc = domainEvent.OccurredOnUtc,
            EventType = "deposit",
            Title = domainEvent.Title,
            Amount = domainEvent.Amount,
            Currency = domainEvent.Currency,
            IsPositive = true
        });
    }

    private async Task ProjectMoneyWithdrawnAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        var domainEvent = Deserialize<MoneyWithdrawnDomainEvent>(message.Payload);
        var productTile = await GetRequiredProductTileAsync(domainEvent.AccountId, cancellationToken);

        productTile.Balance -= domainEvent.Amount;

        _dbContext.TimelineEvents.Add(new TimelineEventReadModel
        {
            Id = message.Id,
            CustomerId = domainEvent.CustomerId,
            ProductId = domainEvent.AccountId,
            EventDateUtc = domainEvent.OccurredOnUtc,
            EventType = "withdrawal",
            Title = domainEvent.Title,
            Amount = domainEvent.Amount,
            Currency = domainEvent.Currency,
            IsPositive = false
        });
    }

    private async Task<ProductTileReadModel> GetRequiredProductTileAsync(Guid productId, CancellationToken cancellationToken)
    {
        var productTile = await _dbContext.ProductTiles.FindAsync([productId], cancellationToken);

        return productTile ?? throw new InvalidOperationException($"Product tile for product '{productId}' was not found.");
    }

    private static bool IsEventType<TEvent>(string typeName)
        => string.Equals(typeName, typeof(TEvent).FullName, StringComparison.Ordinal)
            || string.Equals(typeName, typeof(TEvent).Name, StringComparison.Ordinal);

    private static TEvent Deserialize<TEvent>(string payload)
        => JsonSerializer.Deserialize<TEvent>(payload)
            ?? throw new InvalidOperationException($"Unable to deserialize event '{typeof(TEvent).Name}'.");
}
