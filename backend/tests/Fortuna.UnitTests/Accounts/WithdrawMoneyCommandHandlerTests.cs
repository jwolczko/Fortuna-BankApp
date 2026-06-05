using FluentAssertions;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Application.Accounts.Commands.WithdrawMoney;
using Fortuna.Application.Common.Exceptions;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Accounts.Repositories;
using Fortuna.Domain.Customers;
using NSubstitute;
using Xunit;

namespace Fortuna.UnitTests.Accounts;

public sealed class WithdrawMoneyCommandHandlerTests
{
    [Fact]
    public async Task HandleShouldWithdrawMoneyFromExistingBankAccount()
    {
        var bankAccountRepository = Substitute.For<IBankAccountRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var customerId = CustomerId.New();
        var account = BankAccount.Open(
            customerId,
            new AccountNumber("PL001234567890"),
            "Main account",
            1,
            "PLN",
            BankAccountType.Standard);

        account.Deposit(new Money(300m, "PLN"), "Initial balance");
        account.ClearDomainEvents();

        bankAccountRepository.GetByIdAsync(account.Id, Arg.Any<CancellationToken>())
            .Returns(account);

        var sut = new WithdrawMoneyCommandHandler(bankAccountRepository, unitOfWork);

        var result = await sut.Handle(
            new WithdrawMoneyCommand(customerId.Value, account.Id, 120m, "PLN", "ATM"),
            CancellationToken.None);

        result.Should().Be(account.Id);
        account.Balance.Amount.Should().Be(180m);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleShouldThrowWhenBankAccountDoesNotExist()
    {
        var bankAccountRepository = Substitute.For<IBankAccountRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        bankAccountRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((BankAccount?)null);

        var sut = new WithdrawMoneyCommandHandler(bankAccountRepository, unitOfWork);

        var act = () => sut.Handle(
            new WithdrawMoneyCommand(Guid.NewGuid(), Guid.NewGuid(), 120m, "PLN", "ATM"),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
