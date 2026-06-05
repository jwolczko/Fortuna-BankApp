using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Contracts.Accounts;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Accounts.Repositories;
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

public sealed class AccountFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AccountFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task OpenShouldCreateBankAccountForAuthorizedCustomer()
    {
        var customerId = CustomerId.New();
        var customerRepository = new FakeCustomerRepository(
            new Customer(customerId, new FullName("Jan", "Kowalski"), new Email("jan@example.com"), "hashed"));
        var bankAccountRepository = new FakeBankAccountRepository();
        var productRepository = new FakeProductRepository();
        var client = CreateClient(customerRepository, bankAccountRepository, productRepository);

        var response = await client.PostAsJsonAsync(
            "/api/accounts",
            new OpenBankAccountRequest("IGNORED", "Main account", "PLN", (int)BankAccountType.Standard));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        bankAccountRepository.AddedAccounts.Should().ContainSingle();
        bankAccountRepository.AddedAccounts[0].CustomerId.Should().Be(customerId);
        bankAccountRepository.AddedAccounts[0].AccountName.Should().Be("Main account");
        bankAccountRepository.AddedAccounts[0].AccountType.Should().Be(BankAccountType.Standard);
    }

    [Fact]
    public async Task WithdrawShouldReduceBalanceForAuthorizedCustomer()
    {
        var customerId = CustomerId.New();
        var account = BankAccount.Open(
            customerId,
            new AccountNumber("PL001234567890"),
            "Main account",
            1,
            "PLN",
            BankAccountType.Standard);
        account.Deposit(new Money(300m, "PLN"), "Initial");

        var customerRepository = new FakeCustomerRepository(
            new Customer(customerId, new FullName("Jan", "Kowalski"), new Email("jan@example.com"), "hashed"));
        var bankAccountRepository = new FakeBankAccountRepository(account);
        var productRepository = new FakeProductRepository();
        var client = CreateClient(customerRepository, bankAccountRepository, productRepository);

        var response = await client.PostAsJsonAsync(
            $"/api/accounts/{account.Id}/withdraw",
            new WithdrawMoneyRequest(120m, "PLN", "ATM"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        account.Balance.Amount.Should().Be(180m);
    }

    private HttpClient CreateClient(
        FakeCustomerRepository customerRepository,
        FakeBankAccountRepository bankAccountRepository,
        FakeProductRepository productRepository)
    {
        var customerId = customerRepository.Customers.Single().Id.Value;

        var client = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<ICustomerRepository>();
                    services.RemoveAll<IBankAccountRepository>();
                    services.RemoveAll<IProductRepository>();
                    services.RemoveAll<IUnitOfWork>();
                    services.AddSingleton<ICustomerRepository>(customerRepository);
                    services.AddSingleton<IBankAccountRepository>(bankAccountRepository);
                    services.AddSingleton<IProductRepository>(productRepository);
                    services.AddSingleton<IUnitOfWork>(new FakeUnitOfWork());
                });
            })
            .CreateClient();

        TestAuthentication.Authorize(client, customerId);

        return client;
    }

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        public List<Customer> Customers { get; } = [];

        public FakeCustomerRepository(Customer customer)
        {
            Customers.Add(customer);
        }

        public Task AddAsync(Customer customer, CancellationToken cancellationToken)
        {
            Customers.Add(customer);
            return Task.CompletedTask;
        }

        public Task<Customer?> GetByIdAsync(CustomerId customerId, CancellationToken cancellationToken)
            => Task.FromResult<Customer?>(Customers.FirstOrDefault(x => x.Id == customerId));

        public Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
            => Task.FromResult<Customer?>(Customers.FirstOrDefault(x => x.Email.Value == email.Value));
    }

    private sealed class FakeBankAccountRepository : IBankAccountRepository
    {
        public List<BankAccount> AddedAccounts { get; } = [];

        public FakeBankAccountRepository(params BankAccount[] accounts)
        {
            AddedAccounts.AddRange(accounts);
        }

        public Task AddAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        {
            AddedAccounts.Add(bankAccount);
            return Task.CompletedTask;
        }

        public Task<BankAccount?> GetByIdAsync(Guid bankAccountId, CancellationToken cancellationToken)
            => Task.FromResult<BankAccount?>(AddedAccounts.FirstOrDefault(x => x.Id == bankAccountId));
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private long _sequence;

        public Task AddAsync(Product product, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
            => Task.FromResult<Product?>(null);

        public Task<long> GetNextNumberSequenceAsync(CancellationToken cancellationToken)
            => Task.FromResult(++_sequence);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => Task.FromResult(1);
    }
}
