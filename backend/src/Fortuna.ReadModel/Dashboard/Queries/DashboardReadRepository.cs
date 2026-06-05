using Fortuna.Application.Dashboard.Queries.GetDashboard;
using Fortuna.ReadModel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fortuna.ReadModel.Dashboard.Queries;

public sealed class DashboardReadRepository : IDashboardReadRepository
{
    private readonly ReadDbContext _dbContext;

    public DashboardReadRepository(ReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardDto> GetDashboardAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var products = await _dbContext.ProductTiles
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderBy(x => x.ProductCategory)
            .ThenBy(x => x.ProductName)
            .Select(x => new ProductTileDto(
                x.ProductId,
                x.ProductCategory,
                x.ProductType,
                x.ProductName,
                x.ProductNumber,
                x.Balance,
                x.Currency))
            .ToListAsync(cancellationToken);

        var events = await _dbContext.TimelineEvents
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.EventDateUtc)
            .Take(20)
            .Select(x => new TimelineEventDto(
                x.Id,
                x.EventDateUtc,
                x.EventType,
                x.Title,
                x.Amount,
                x.Currency,
                x.IsPositive))
            .ToListAsync(cancellationToken);

        var totalBalance = products.Sum(x => x.Balance);
        var currency = products.FirstOrDefault()?.Currency ?? "PLN";

        return new DashboardDto(customerId, totalBalance, currency, products, events);
    }
}
