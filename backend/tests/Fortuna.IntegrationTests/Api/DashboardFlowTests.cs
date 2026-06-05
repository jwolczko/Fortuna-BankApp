using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Fortuna.Application.Dashboard.Queries.GetDashboard;
using Fortuna.Contracts.Dashboard;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Fortuna.IntegrationTests.Api;

public sealed class DashboardFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DashboardFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetShouldReturnDashboardForAuthenticatedCustomer()
    {
        var customerId = Guid.NewGuid();
        var repository = new FakeDashboardReadRepository(
            new DashboardDto(
                customerId,
                450m,
                "PLN",
                [new ProductTileDto(Guid.NewGuid(), "BankAccount", "Standard", "Main", "PL001", 450m, "PLN")],
                [new TimelineEventDto(Guid.NewGuid(), DateTime.UtcNow, "deposit", "Salary", 450m, "PLN", true)]));
        var client = CreateClient(customerId, repository);

        var response = await client.GetAsync($"/api/dashboard/{customerId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DashboardResponse>();

        body.Should().NotBeNull();
        body!.CustomerId.Should().Be(customerId);
        body.TotalBalance.Should().Be(450m);
        body.Products.Should().ContainSingle(x => x.ProductName == "Main");
        body.Events.Should().ContainSingle(x => x.EventType == "deposit");
    }

    [Fact]
    public async Task GetShouldReturnForbiddenWhenCustomerRequestsAnotherDashboard()
    {
        var authenticatedCustomerId = Guid.NewGuid();
        var requestedCustomerId = Guid.NewGuid();
        var repository = new FakeDashboardReadRepository(
            new DashboardDto(requestedCustomerId, 0m, "PLN", [], []));
        var client = CreateClient(authenticatedCustomerId, repository);

        var response = await client.GetAsync($"/api/dashboard/{requestedCustomerId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        repository.WasCalled.Should().BeFalse();
    }

    private HttpClient CreateClient(Guid customerId, FakeDashboardReadRepository repository)
    {
        var client = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IDashboardReadRepository>();
                    services.AddSingleton<IDashboardReadRepository>(repository);
                });
            })
            .CreateClient();

        TestAuthentication.Authorize(client, customerId);

        return client;
    }

    private sealed class FakeDashboardReadRepository : IDashboardReadRepository
    {
        private readonly DashboardDto _result;

        public FakeDashboardReadRepository(DashboardDto result)
        {
            _result = result;
        }

        public bool WasCalled { get; private set; }

        public Task<DashboardDto> GetDashboardAsync(Guid customerId, CancellationToken cancellationToken)
        {
            WasCalled = true;
            return Task.FromResult(_result);
        }
    }
}
