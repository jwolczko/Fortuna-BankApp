using System.Reflection;
using System.Text.Json;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Domain.Abstractions;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Cards;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Loans;
using Fortuna.Domain.Products;
using Fortuna.Domain.Transfers;
using Microsoft.EntityFrameworkCore;

namespace Fortuna.Infrastructure.Persistence.Write;

public sealed class WriteDbContext : DbContext, IUnitOfWork
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<TransactionEntry> Transactions => Set<TransactionEntry>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = ChangeTracker
            .Entries()
            .Select(e => e.Entity)
            .Where(entity => entity is not null)
            .Select(entity => new
            {
                Entity = entity!,
                DomainEventsProperty = entity!.GetType().GetProperty("DomainEvents", BindingFlags.Instance | BindingFlags.Public)
            })
            .Where(x => x.DomainEventsProperty is not null)
            .ToList();

        var events = entitiesWithEvents
            .SelectMany(x => x.DomainEventsProperty!.GetValue(x.Entity) as IReadOnlyCollection<IDomainEvent> ?? [])
            .ToList();

        foreach (var domainEvent in events)
        {
            OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
                Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                OccurredOnUtc = domainEvent.OccurredOnUtc
            });
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var item in entitiesWithEvents)
        {
            var clearMethod = item.Entity.GetType().GetMethod("ClearDomainEvents", BindingFlags.Instance | BindingFlags.Public);
            clearMethod?.Invoke(item.Entity, null);
        }

        return result;
    }
}
