using Fortuna.Domain.Abstractions;
using Fortuna.Domain.Accounts.Events;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Products;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fortuna.Domain.Accounts;

public sealed class BankAccount : Product, IAggregateRoot
{
    private readonly List<TransactionEntry> _transactions = [];

    private BankAccount()
    {
    }

    private BankAccount(
        Guid id,
        CustomerId customerId,
        AccountNumber accountNumber,
        string accountName,
        long numberSequence,
        string currency,
        BankAccountType accountType) : base(
        id,
        customerId,
        accountName,
        accountNumber.Value,
        numberSequence,
        currency,
        ProductCategory.BankAccount,
        ProductStatus.Active)
    {
        AccountNumber = accountNumber;
        AccountType = accountType;
        RaiseCreatedDomainEvent(accountType.ToString());
    }

    public AccountNumber AccountNumber { get; private set; } = default!;
    [NotMapped]
    public string AccountName => ProductName;

    [NotMapped]
    public new AccountStatus Status => (AccountStatus)(int)base.Status;
    public BankAccountType AccountType { get; private set; }
    public IReadOnlyCollection<TransactionEntry> Transactions => _transactions;

    public static BankAccount Open(
        CustomerId customerId,
        AccountNumber accountNumber,
        string accountName,
        long numberSequence,
        string currency,
        BankAccountType accountType)
        => new(Guid.NewGuid(), customerId, accountNumber, accountName, numberSequence, currency, accountType);

    public void Deposit(Money amount, string title, Guid? transferId = null)
    {
        EnsureActive("Account is not active.");
        EnsurePositive(amount);

        SetBalance(Balance.Add(amount));
        _transactions.Add(TransactionEntry.CreateCredit(Id, amount, title, transferId));

        AddDomainEvent(new MoneyDepositedDomainEvent(
            Id,
            CustomerId.Value,
            amount.Amount,
            amount.Currency,
            title));
    }

    public void Withdraw(Money amount, string title, Guid? transferId = null)
    {
        EnsureActive("Account is not active.");
        EnsurePositive(amount);

        if (Balance.Amount < amount.Amount)
            throw new DomainException("Insufficient funds.");

        SetBalance(Balance.Subtract(amount));
        _transactions.Add(TransactionEntry.CreateDebit(Id, amount, title, transferId));

        AddDomainEvent(new MoneyWithdrawnDomainEvent(
            Id,
            CustomerId.Value,
            amount.Amount,
            amount.Currency,
            title));
    }
}
