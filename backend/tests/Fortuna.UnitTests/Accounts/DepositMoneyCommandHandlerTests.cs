using FluentAssertions;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Application.Accounts.Commands.DepositMoney;
using Fortuna.Domain.Accounts.Events;
using Fortuna.Domain.Cards;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Products.Repositories;
using NSubstitute;
using Xunit;

namespace Fortuna.UnitTests.Accounts;

public sealed class DepositMoneyCommandHandlerTests
{
    [Fact]
    public async Task HandleShouldDepositMoneyToDebitCard()
    {
        var productRepository = Substitute.For<IProductRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var customerId = CustomerId.New();
        var debitCard = Card.Create(customerId, "Debit Card", "CARD-001", 1, "PLN", CardType.Debit);

        productRepository.GetByIdAsync(debitCard.Id, Arg.Any<CancellationToken>())
            .Returns(debitCard);

        var sut = new DepositMoneyCommandHandler(productRepository, unitOfWork);

        var result = await sut.Handle(
            new DepositMoneyCommand(debitCard.Id, 250m, "PLN", "Incoming transfer"),
            CancellationToken.None);

        result.Should().Be(debitCard.Id);
        debitCard.Balance.Amount.Should().Be(250m);
        debitCard.DomainEvents.Should().Contain(x => x is MoneyDepositedDomainEvent);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
