using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Contracts.Customers;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Cards;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Customers.Repositories;
using Fortuna.Domain.Products;
using Fortuna.Domain.Products.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Fortuna.IntegrationTests.Api;

public sealed class CustomerRegistrationFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CustomerRegistrationFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterShouldCreateStandardAccountAndDebitCardForNormalCustomer()
    {
        var customerRepository = new FakeCustomerRepository();
        var productRepository = new FakeProductRepository();
        var client = CreateClient(customerRepository, productRepository);

        var response = await client.PostAsJsonAsync(
            "/api/customers",
            new RegisterCustomerRequest("Jan", "Kowalski", "jan@example.com", "Secret123!", "Norma"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<RegisterCustomerResponse>();

        body.Should().NotBeNull();
        body!.CustomerId.Should().NotBeEmpty();
        customerRepository.AddedCustomers.Should().ContainSingle(x => x.Type == CustomerType.Normal);
        productRepository.AddedProducts.Should().HaveCount(2);
        productRepository.AddedProducts.OfType<BankAccount>()
            .Should().ContainSingle(x => x.AccountType == BankAccountType.Standard);
        productRepository.AddedProducts.OfType<Card>()
            .Should().ContainSingle(x => x.CardType == CardType.Debit && x.CreditLimit == null);
    }

    [Fact]
    public async Task RegisterShouldCreatePrestigeAccountDebitCardAndCreditCardForPrestigeCustomer()
    {
        var customerRepository = new FakeCustomerRepository();
        var productRepository = new FakeProductRepository();
        var client = CreateClient(customerRepository, productRepository);

        var response = await client.PostAsJsonAsync(
            "/api/customers",
            new RegisterCustomerRequest("Anna", "Nowak", "anna@example.com", "Secret123!", "Prestige"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        customerRepository.AddedCustomers.Should().ContainSingle(x => x.Type == CustomerType.Prestige);
        productRepository.AddedProducts.Should().HaveCount(3);
        productRepository.AddedProducts.OfType<BankAccount>()
            .Should().ContainSingle(x => x.AccountType == BankAccountType.Prestige);
        productRepository.AddedProducts.OfType<Card>()
            .Should().ContainSingle(x => x.CardType == CardType.Debit && x.CreditLimit == null);
        productRepository.AddedProducts.OfType<Card>()
            .Should().ContainSingle(x => x.CardType == CardType.Credit && x.CreditLimit == 10000m);
    }

    [Fact]
    public async Task RegisterShouldReturnBadRequestWhenCustomerTypeIsInvalid()
    {
        var customerRepository = new FakeCustomerRepository();
        var productRepository = new FakeProductRepository();
        var client = CreateClient(customerRepository, productRepository);

        var response = await client.PostAsJsonAsync(
            "/api/customers",
            new RegisterCustomerRequest("Anna", "Nowak", "anna@example.com", "Secret123!", "Vip"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        body.Should().NotBeNull();
        body!.Error.Should().Be("Customer type must be 'Normal' or 'Prestige'.");
        customerRepository.AddedCustomers.Should().BeEmpty();
        productRepository.AddedProducts.Should().BeEmpty();
    }

    private HttpClient CreateClient(FakeCustomerRepository customerRepository, FakeProductRepository productRepository)
        => _factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<ICustomerRepository>();
                    services.RemoveAll<IProductRepository>();
                    services.RemoveAll<IUnitOfWork>();
                    services.AddSingleton<ICustomerRepository>(customerRepository);
                    services.AddSingleton<IProductRepository>(productRepository);
                    services.AddSingleton<IUnitOfWork>(new FakeUnitOfWork());
                });
            })
            .CreateClient();

    private sealed record ErrorResponse(string Error);

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        public List<Customer> AddedCustomers { get; } = [];

        public Task AddAsync(Customer customer, CancellationToken cancellationToken)
        {
            AddedCustomers.Add(customer);
            return Task.CompletedTask;
        }

        public Task<Customer?> GetByIdAsync(CustomerId customerId, CancellationToken cancellationToken)
            => Task.FromResult<Customer?>(AddedCustomers.FirstOrDefault(x => x.Id == customerId));

        public Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
            => Task.FromResult<Customer?>(AddedCustomers.FirstOrDefault(x => x.Email.Value == email.Value));
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> AddedProducts { get; } = [];
        private long _sequence;

        public Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            AddedProducts.Add(product);
            return Task.CompletedTask;
        }

        public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
            => Task.FromResult<Product?>(AddedProducts.FirstOrDefault(x => x.Id == productId));

        public Task<long> GetNextNumberSequenceAsync(CancellationToken cancellationToken)
            => Task.FromResult(++_sequence);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => Task.FromResult(1);
    }
}
