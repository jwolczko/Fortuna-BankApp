using FluentAssertions;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Application.Accounts.Commands.OpenBankAccount;
using Fortuna.Application.Common.Exceptions;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Accounts.Repositories;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Customers.Repositories;
using Fortuna.Domain.Products.Repositories;
using NSubstitute;
using Xunit;

namespace Fortuna.UnitTests.Accounts;

public sealed class OpenBankAccountCommandHandlerTests
{
    [Fact]
    public async Task HandleShouldCreateBankAccountForExistingCustomer()
    {
        var customerRepository = Substitute.For<ICustomerRepository>();
        var bankAccountRepository = Substitute.For<IBankAccountRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        BankAccount? addedBankAccount = null;
        var customerId = CustomerId.New();
        var customer = new Customer(customerId, new FullName("Jan", "Kowalski"), new Email("jan@example.com"), "hashed");

        customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        productRepository.GetNextNumberSequenceAsync(Arg.Any<CancellationToken>())
            .Returns(7L);
        bankAccountRepository.AddAsync(Arg.Do<BankAccount>(account => addedBankAccount = account), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var sut = new OpenBankAccountCommandHandler(customerRepository, bankAccountRepository, productRepository, unitOfWork);

        var result = await sut.Handle(
            new OpenBankAccountCommand(customerId.Value, "IGNORED", "Main account", "PLN", (int)BankAccountType.Standard),
            CancellationToken.None);

        result.Should().NotBeEmpty();
        addedBankAccount.Should().NotBeNull();
        addedBankAccount!.CustomerId.Should().Be(customerId);
        addedBankAccount.AccountType.Should().Be(BankAccountType.Standard);
        addedBankAccount.AccountName.Should().Be("Main account");
        addedBankAccount.AccountNumber.Value.Should().Be("13696969690000000000000007");
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleShouldThrowWhenCustomerDoesNotExist()
    {
        var customerRepository = Substitute.For<ICustomerRepository>();
        var bankAccountRepository = Substitute.For<IBankAccountRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var customerId = Guid.NewGuid();

        customerRepository.GetByIdAsync(Arg.Any<CustomerId>(), Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        var sut = new OpenBankAccountCommandHandler(customerRepository, bankAccountRepository, productRepository, unitOfWork);

        var act = () => sut.Handle(
            new OpenBankAccountCommand(customerId, "IGNORED", "Main account", "PLN", (int)BankAccountType.Standard),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        await bankAccountRepository.DidNotReceive().AddAsync(Arg.Any<BankAccount>(), Arg.Any<CancellationToken>());
    }
}
