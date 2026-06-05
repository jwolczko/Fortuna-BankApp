using FluentAssertions;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Application.Transfers.Commands.CreateTransfer;
using Fortuna.Application.Common.Exceptions;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Accounts.Events;
using Fortuna.Domain.Cards;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Products.Repositories;
using Fortuna.Domain.Transfers.Repositories;
using NSubstitute;
using Xunit;

namespace Fortuna.UnitTests.Transfers;

public sealed class CreateTransferCommandHandlerTests
{
    [Fact]
    public async Task HandleShouldWithdrawFromSourceAccountAndDepositToDebitCardForOwnTransfer()
    {
        var productRepository = Substitute.For<IProductRepository>();
        var transferRepository = Substitute.For<ITransferRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var customerId = CustomerId.New();
        var sourceAccount = BankAccount.Open(
            customerId,
            new AccountNumber("PL001234567890"),
            "Main account",
            1,
            "PLN",
            BankAccountType.Standard);
        var targetCard = Card.Create(customerId, "Debit Card", "CARD-001", 2, "PLN", CardType.Debit);

        sourceAccount.Deposit(new Money(500m, "PLN"), "Initial balance");
        sourceAccount.ClearDomainEvents();
        targetCard.ClearDomainEvents();

        productRepository.GetByIdAsync(sourceAccount.Id, Arg.Any<CancellationToken>())
            .Returns(sourceAccount);
        productRepository.GetByIdAsync(targetCard.Id, Arg.Any<CancellationToken>())
            .Returns(targetCard);

        var sut = new CreateTransferCommandHandler(productRepository, transferRepository, unitOfWork);

        var transferId = await sut.Handle(
            new CreateTransferCommand(
                "Own",
                customerId.Value,
                sourceAccount.Id,
                targetCard.Id,
                null,
                null,
                125m,
                "PLN",
                "Top up card"),
            CancellationToken.None);

        transferId.Should().NotBeEmpty();
        sourceAccount.Balance.Amount.Should().Be(375m);
        targetCard.Balance.Amount.Should().Be(125m);
        sourceAccount.DomainEvents.Should().Contain(x => x is MoneyWithdrawnDomainEvent);
        targetCard.DomainEvents.Should().Contain(x => x is MoneyDepositedDomainEvent);
        await transferRepository.Received(1).AddAsync(Arg.Any<Fortuna.Domain.Transfers.Transfer>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleShouldWithdrawFromSourceAccountForExternalTransfer()
    {
        var productRepository = Substitute.For<IProductRepository>();
        var transferRepository = Substitute.For<ITransferRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var customerId = CustomerId.New();
        var sourceAccount = BankAccount.Open(
            customerId,
            new AccountNumber("PL001234567890"),
            "Main account",
            1,
            "PLN",
            BankAccountType.Standard);

        sourceAccount.Deposit(new Money(500m, "PLN"), "Initial balance");
        sourceAccount.ClearDomainEvents();

        productRepository.GetByIdAsync(sourceAccount.Id, Arg.Any<CancellationToken>())
            .Returns(sourceAccount);

        var sut = new CreateTransferCommandHandler(productRepository, transferRepository, unitOfWork);

        var transferId = await sut.Handle(
            new CreateTransferCommand(
                "External",
                customerId.Value,
                sourceAccount.Id,
                null,
                "PL88999999999999999999999999",
                "Receiver",
                200m,
                "PLN",
                "Rent"),
            CancellationToken.None);

        transferId.Should().NotBeEmpty();
        sourceAccount.Balance.Amount.Should().Be(300m);
        sourceAccount.DomainEvents.Should().Contain(x => x is MoneyWithdrawnDomainEvent);
        await transferRepository.Received(1).AddAsync(Arg.Any<Fortuna.Domain.Transfers.Transfer>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleShouldThrowWhenTargetProductIsMissingForOwnTransfer()
    {
        var productRepository = Substitute.For<IProductRepository>();
        var transferRepository = Substitute.For<ITransferRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var customerId = CustomerId.New();
        var sourceAccount = BankAccount.Open(
            customerId,
            new AccountNumber("PL001234567890"),
            "Main account",
            1,
            "PLN",
            BankAccountType.Standard);

        sourceAccount.Deposit(new Money(500m, "PLN"), "Initial balance");

        productRepository.GetByIdAsync(sourceAccount.Id, Arg.Any<CancellationToken>())
            .Returns(sourceAccount);
        productRepository.GetByIdAsync(Arg.Is<Guid>(id => id != sourceAccount.Id), Arg.Any<CancellationToken>())
            .Returns((Fortuna.Domain.Products.Product?)null);

        var sut = new CreateTransferCommandHandler(productRepository, transferRepository, unitOfWork);

        var act = () => sut.Handle(
            new CreateTransferCommand(
                "Own",
                customerId.Value,
                sourceAccount.Id,
                Guid.NewGuid(),
                null,
                null,
                50m,
                "PLN",
                "Transfer"),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
