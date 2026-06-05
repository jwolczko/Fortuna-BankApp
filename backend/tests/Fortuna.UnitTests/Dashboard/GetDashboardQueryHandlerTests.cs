using FluentAssertions;
using Fortuna.Application.Dashboard.Queries.GetDashboard;
using NSubstitute;
using Xunit;

namespace Fortuna.UnitTests.Dashboard;

public sealed class GetDashboardQueryHandlerTests
{
    [Fact]
    public async Task HandleShouldReturnDashboardFromRepository()
    {
        var repository = Substitute.For<IDashboardReadRepository>();
        var customerId = Guid.NewGuid();
        var expected = new DashboardDto(
            customerId,
            250m,
            "PLN",
            [new ProductTileDto(Guid.NewGuid(), "BankAccount", "Standard", "Main", "PL001", 250m, "PLN")],
            []);

        repository.GetDashboardAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(expected);

        var sut = new GetDashboardQueryHandler(repository);

        var result = await sut.Handle(new GetDashboardQuery(customerId), CancellationToken.None);

        result.Should().BeSameAs(expected);
        await repository.Received(1).GetDashboardAsync(customerId, Arg.Any<CancellationToken>());
    }
}
