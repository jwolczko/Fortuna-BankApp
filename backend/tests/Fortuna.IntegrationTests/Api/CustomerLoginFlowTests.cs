using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Fortuna.Contracts.Customers;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Customers.Repositories;
using Fortuna.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Fortuna.IntegrationTests.Api;

public sealed class CustomerLoginFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CustomerLoginFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task LoginShouldReturnJwtWhenCredentialsAreValid()
    {
        var password = "Secret123!";
        var hasher = new Pbkdf2PasswordHasher();
        var customer = new Customer(
            CustomerId.New(),
            new FullName("Jan", "Kowalski"),
            new Email("jan@example.com"),
            hasher.Hash(password));

        var client = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<ICustomerRepository>();
                    services.AddSingleton<ICustomerRepository>(new FakeCustomerRepository(customer));
                });
            })
            .CreateClient();

        var response = await client.PostAsJsonAsync("/api/customers/login", new LoginCustomerRequest(customer.Email.Value, password));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<LoginCustomerResponse>();

        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow.AddMinutes(4));
        body.ExpiresAtUtc.Should().BeBefore(DateTime.UtcNow.AddMinutes(6));
    }

    [Fact]
    public async Task LoginShouldReturnUnauthorizedWhenPasswordIsInvalid()
    {
        var password = "Secret123!";
        var hasher = new Pbkdf2PasswordHasher();
        var customer = new Customer(
            CustomerId.New(),
            new FullName("Jan", "Kowalski"),
            new Email("jan@example.com"),
            hasher.Hash(password));

        var client = CreateClientWithRepository(new FakeCustomerRepository(customer));

        var response = await client.PostAsJsonAsync("/api/customers/login", new LoginCustomerRequest(customer.Email.Value, "WrongPassword"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        body.Should().NotBeNull();
        body!.Error.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task LoginShouldReturnUnauthorizedWhenCustomerDoesNotExist()
    {
        var client = CreateClientWithRepository(new FakeCustomerRepository(null));

        var response = await client.PostAsJsonAsync("/api/customers/login", new LoginCustomerRequest("missing@example.com", "Secret123!"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        body.Should().NotBeNull();
        body!.Error.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task LoginShouldReturnBadRequestWhenEmailIsInvalid()
    {
        var client = CreateClientWithRepository(new FakeCustomerRepository(null));

        var response = await client.PostAsJsonAsync("/api/customers/login", new LoginCustomerRequest("not-an-email", "Secret123!"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        body.Should().NotBeNull();
        body!.Error.Should().Be("Invalid email address.");
    }

    private HttpClient CreateClientWithRepository(ICustomerRepository repository)
        => _factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<ICustomerRepository>();
                    services.AddSingleton(repository);
                });
            })
            .CreateClient();

    private sealed record ErrorResponse(string Error);

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        private readonly Customer? _customer;

        public FakeCustomerRepository(Customer? customer)
        {
            _customer = customer;
        }

        public Task AddAsync(Customer customer, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<Customer?> GetByIdAsync(CustomerId customerId, CancellationToken cancellationToken)
            => Task.FromResult<Customer?>(_customer?.Id == customerId ? _customer : null);

        public Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
            => Task.FromResult<Customer?>(_customer?.Email.Equals(email) == true ? _customer : null);
    }
}
