using FluentAssertions;
using Fortuna.ReadModel.Dashboard.Queries;
using Fortuna.ReadModel.Dashboard.ReadModels;
using Fortuna.ReadModel.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fortuna.UnitTests.ReadModel;

public sealed class DashboardReadRepositoryTests
{
    [Fact]
    public async Task GetDashboardAsyncShouldReturnAggregatedProductsAndEvents()
    {
        await using var dbContext = CreateDbContext();
        var customerId = Guid.NewGuid();

        dbContext.ProductTiles.AddRange(
            new ProductTileReadModel
            {
                ProductId = Guid.NewGuid(),
                CustomerId = customerId,
                ProductCategory = "Account",
                ProductType = "Checking",
                ProductName = "Daily",
                ProductNumber = "PL001",
                Balance = 50m,
                Currency = "PLN"
            },
            new ProductTileReadModel
            {
                ProductId = Guid.NewGuid(),
                CustomerId = customerId,
                ProductCategory = "Account",
                ProductType = "Savings",
                ProductName = "Savings",
                ProductNumber = "PL002",
                Balance = 150m,
                Currency = "PLN"
            });

        dbContext.TimelineEvents.Add(new TimelineEventReadModel
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            EventDateUtc = DateTime.UtcNow,
            EventType = "deposit",
            Title = "Salary",
            Amount = 200m,
            Currency = "PLN",
            IsPositive = true
        });

        await dbContext.SaveChangesAsync();

        var sut = new DashboardReadRepository(dbContext);

        var result = await sut.GetDashboardAsync(customerId, CancellationToken.None);

        result.TotalBalance.Should().Be(200m);
        result.Currency.Should().Be("PLN");
        result.Products.Should().HaveCount(2);
        result.Products.Select(x => x.ProductName).Should().ContainInOrder("Daily", "Savings");
        result.Events.Should().ContainSingle();
    }

    [Fact]
    public async Task GetDashboardAsyncShouldReturnActualBalanceForDebitCard()
    {
        await using var dbContext = CreateDbContext();
        var customerId = Guid.NewGuid();

        dbContext.ProductTiles.Add(new ProductTileReadModel
        {
            ProductId = Guid.NewGuid(),
            CustomerId = customerId,
            ProductCategory = "Card",
            ProductType = "Debit",
            ProductName = "Debit Card",
            ProductNumber = "CARD001",
            Balance = 125m,
            Currency = "PLN"
        });

        await dbContext.SaveChangesAsync();

        var sut = new DashboardReadRepository(dbContext);

        var result = await sut.GetDashboardAsync(customerId, CancellationToken.None);

        result.TotalBalance.Should().Be(125m);
        result.Products.Should().ContainSingle();
        result.Products.Single().Balance.Should().Be(125m);
    }

    private static ReadDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ReadDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ReadDbContext(options);
    }
}
