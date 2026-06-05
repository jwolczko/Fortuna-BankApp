using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Contracts.Transfers;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Cards;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Products;
using Fortuna.Domain.Products.Repositories;
using Fortuna.Domain.Transfers;
using Fortuna.Domain.Transfers.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Fortuna.IntegrationTests.Api;

public sealed class TransferFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TransferFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateShouldMoveMoneyFromAccountToDebitCard()
    {
        var customerId = CustomerId.New();
        var sourceAccount = BankAccount.Open(
            customerId,
            new AccountNumber("PL001234567890"),
            "Main account",
            1,
            "PLN",
            BankAccountType.Standard);
        var targetCard = Card.Create(customerId, "Debit Card", "CARD0001", 2, "PLN", CardType.Debit);
        sourceAccount.Deposit(new Money(400m, "PLN"), "Initial");

        var productRepository = new FakeProductRepository(sourceAccount, targetCard);
        var transferRepository = new FakeTransferRepository();
        var client = CreateClient(customerId.Value, productRepository, transferRepository);

        var response = await client.PostAsJsonAsync(
            "/api/transfers",
            new CreateTransferRequest("Own", sourceAccount.Id, targetCard.Id, null, null, 150m, "PLN", "Top up"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sourceAccount.Balance.Amount.Should().Be(250m);
        targetCard.Balance.Amount.Should().Be(150m);
        transferRepository.AddedTransfers.Should().ContainSingle();
    }

    [Fact]
    public async Task IncomingShouldDepositMoneyToDebitCard()
    {
        var customerId = CustomerId.New();
        var targetCard = Card.Create(customerId, "Debit Card", "CARD0001", 2, "PLN", CardType.Debit);
        var productRepository = new FakeProductRepository(targetCard);
        var transferRepository = new FakeTransferRepository();
        var client = CreateClient(customerId.Value, productRepository, transferRepository);

        var response = await client.PostAsJsonAsync(
            "/api/transfers/incoming",
            new IncomingTransferRequest(targetCard.Id, 90m, "PLN", "Incoming"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        targetCard.Balance.Amount.Should().Be(90m);
    }

    private HttpClient CreateClient(
        Guid customerId,
        FakeProductRepository productRepository,
        FakeTransferRepository transferRepository)
    {
        var client = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IProductRepository>();
                    services.RemoveAll<ITransferRepository>();
                    services.RemoveAll<IUnitOfWork>();
                    services.AddSingleton<IProductRepository>(productRepository);
                    services.AddSingleton<ITransferRepository>(transferRepository);
                    services.AddSingleton<IUnitOfWork>(new FakeUnitOfWork());
                });
            })
            .CreateClient();

        TestAuthentication.Authorize(client, customerId);

        return client;
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products;

        public FakeProductRepository(params Product[] products)
        {
            _products = products.ToList();
        }

        public Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
            => Task.FromResult<Product?>(_products.FirstOrDefault(x => x.Id == productId));

        public Task<long> GetNextNumberSequenceAsync(CancellationToken cancellationToken)
            => Task.FromResult(1L);
    }

    private sealed class FakeTransferRepository : ITransferRepository
    {
        public List<Transfer> AddedTransfers { get; } = [];

        public Task AddAsync(Transfer transfer, CancellationToken cancellationToken)
        {
            AddedTransfers.Add(transfer);
            return Task.CompletedTask;
        }

        public Task<Transfer?> GetByIdAsync(TransferId transferId, CancellationToken cancellationToken)
            => Task.FromResult<Transfer?>(AddedTransfers.FirstOrDefault(x => x.Id == transferId));
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => Task.FromResult(1);
    }
}
