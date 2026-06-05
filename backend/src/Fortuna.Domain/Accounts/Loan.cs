using Fortuna.Domain.Abstractions;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Products;

namespace Fortuna.Domain.Loans;

public sealed class Loan : Product, IAggregateRoot
{
    private Loan()
    {
    }

    private Loan(
        Guid id,
        CustomerId customerId,
        string productName,
        string productNumber,
        long numberSequence,
        string currency,
        LoanType loanType,
        decimal initialBalance) : base(
        id,
        customerId,
        productName,
        productNumber,
        numberSequence,
        currency,
        ProductCategory.Loan,
        ProductStatus.Active)
    {
        LoanType = loanType;
        SetBalance(new Money(initialBalance, currency));
        RaiseCreatedDomainEvent(loanType.ToString());
    }

    public LoanType LoanType { get; private set; }

    public static Loan Create(
        CustomerId customerId,
        string productName,
        string productNumber,
        long numberSequence,
        string currency,
        LoanType loanType,
        decimal initialBalance)
        => new(Guid.NewGuid(), customerId, productName, productNumber, numberSequence, currency, loanType, initialBalance);
}
