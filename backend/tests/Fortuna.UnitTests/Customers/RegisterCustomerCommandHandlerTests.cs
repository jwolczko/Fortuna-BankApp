using FluentAssertions;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Application.Abstractions.Security;
using Fortuna.Application.Customers.Commands.RegisterCustomer;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Cards;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Customers.Repositories;
using Fortuna.Domain.Products;
using Fortuna.Domain.Products.Repositories;
using NSubstitute;
using Xunit;

namespace Fortuna.UnitTests.Customers;

public sealed class RegisterCustomerCommandHandlerTests
{
    [Fact]
    public async Task HandleShouldCreateStandardAccountAndDebitCardForNormalCustomer()
    {
        var customerRepository = Substitute.For<ICustomerRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        Customer? addedCustomer = null;
        var addedProducts = new List<Product>();

        customerRepository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns((Customer?)null);
        customerRepository.AddAsync(Arg.Do<Customer>(customer => addedCustomer = customer), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        productRepository.AddAsync(Arg.Do<Product>(product => addedProducts.Add(product)), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        passwordHasher.Hash("Secret123!").Returns("hashed-password");

        var sut = new RegisterCustomerCommandHandler(customerRepository, productRepository, passwordHasher, unitOfWork);

        var customerId = await sut.Handle(
            new RegisterCustomerCommand("Jan", "Kowalski", "jan@example.com", "Secret123!", "Norma"),
            CancellationToken.None);

        customerId.Should().NotBeEmpty();
        addedCustomer.Should().NotBeNull();
        addedCustomer!.Type.Should().Be(CustomerType.Normal);
        addedProducts.Should().HaveCount(2);
        addedProducts.OfType<BankAccount>().Should().ContainSingle(x => x.AccountType == BankAccountType.Standard);
        addedProducts.OfType<Card>().Should().ContainSingle(x => x.CardType == CardType.Debit && x.CreditLimit == null);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleShouldCreatePrestigeAccountDebitCardAndCreditCardForPrestigeCustomer()
    {
        var customerRepository = Substitute.For<ICustomerRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        Customer? addedCustomer = null;
        var addedProducts = new List<Product>();

        customerRepository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns((Customer?)null);
        customerRepository.AddAsync(Arg.Do<Customer>(customer => addedCustomer = customer), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        productRepository.AddAsync(Arg.Do<Product>(product => addedProducts.Add(product)), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        passwordHasher.Hash("Secret123!").Returns("hashed-password");

        var sut = new RegisterCustomerCommandHandler(customerRepository, productRepository, passwordHasher, unitOfWork);

        var customerId = await sut.Handle(
            new RegisterCustomerCommand("Anna", "Nowak", "anna@example.com", "Secret123!", "Prestige"),
            CancellationToken.None);

        customerId.Should().NotBeEmpty();
        addedCustomer.Should().NotBeNull();
        addedCustomer!.Type.Should().Be(CustomerType.Prestige);
        addedProducts.Should().HaveCount(3);
        addedProducts.OfType<BankAccount>().Should().ContainSingle(x => x.AccountType == BankAccountType.Prestige);
        addedProducts.OfType<Card>().Should().ContainSingle(x => x.CardType == CardType.Debit && x.CreditLimit == null);
        addedProducts.OfType<Card>().Should().ContainSingle(x => x.CardType == CardType.Credit && x.CreditLimit == 10000m);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
