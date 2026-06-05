using Fortuna.Domain.Abstractions;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Accounts.Events;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Products;

namespace Fortuna.Domain.Cards;

public sealed class Card : Product, IAggregateRoot
{
    private Card()
    {
    }

    private Card(
        Guid id,
        CustomerId customerId,
        string productName,
        string productNumber,
        long numberSequence,
        string currency,
        CardType cardType,
        decimal? creditLimit) : base(
        id,
        customerId,
        productName,
        productNumber,
        numberSequence,
        currency,
        ProductCategory.Card,
        ProductStatus.Active)
    {
        if (cardType == CardType.Credit && (!creditLimit.HasValue || creditLimit.Value <= 0))
            throw new DomainException("Credit card limit must be greater than zero.");

        if (cardType == CardType.Debit && creditLimit.HasValue)
            throw new DomainException("Debit card cannot have a credit limit.");

        CardType = cardType;
        CreditLimit = creditLimit;

        if (cardType == CardType.Credit)
        {
            SetBalance(new Money(creditLimit!.Value, currency));
        }

        RaiseCreatedDomainEvent(cardType.ToString());
    }

    public CardType CardType { get; private set; }
    public decimal? CreditLimit { get; private set; }

    public void Deposit(Money amount, string title, Guid? transferId = null)
    {
        EnsureActive("Card is not active.");
        EnsurePositive(amount);

        var newBalance = Balance.Add(amount);
        if (CardType == CardType.Credit && CreditLimit.HasValue && newBalance.Amount > CreditLimit.Value)
            throw new DomainException("Credit card limit cannot be exceeded.");

        SetBalance(newBalance);

        AddDomainEvent(new MoneyDepositedDomainEvent(
            Id,
            CustomerId.Value,
            amount.Amount,
            amount.Currency,
            title));
    }

    public void Withdraw(Money amount, string title, Guid? transferId = null)
    {
        EnsureActive("Card is not active.");
        EnsurePositive(amount);

        if (Balance.Amount < amount.Amount)
            throw new DomainException("Insufficient funds.");

        SetBalance(Balance.Subtract(amount));

        AddDomainEvent(new MoneyWithdrawnDomainEvent(
            Id,
            CustomerId.Value,
            amount.Amount,
            amount.Currency,
            title));
    }

    public static Card Create(
        CustomerId customerId,
        string productName,
        string productNumber,
        long numberSequence,
        string currency,
        CardType cardType,
        decimal? creditLimit = null)
        => new(Guid.NewGuid(), customerId, productName, productNumber, numberSequence, currency, cardType, creditLimit);
}
