using Fortuna.Domain.Abstractions;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Products.Events;

namespace Fortuna.Domain.Products;

public abstract class Product : Entity<Guid>
{
    protected Product()
    {
    }

    protected Product(
        Guid id,
        CustomerId customerId,
        string productName,
        string productNumber,
        long numberSequence,
        string currency,
        ProductCategory category,
        ProductStatus status) : base(id)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("Product name is required.");

        if (string.IsNullOrWhiteSpace(productNumber))
            throw new DomainException("Product number is required.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required.");

        CustomerId = customerId;
        ProductName = productName.Trim();
        ProductNumber = productNumber.Replace(" ", string.Empty).Trim();
        NumberSequence = numberSequence;
        Currency = currency.Trim().ToUpperInvariant();
        Category = category;
        Status = status;
        Balance = Money.Zero(Currency);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public CustomerId CustomerId { get; protected set; } = default!;
    public string ProductName { get; protected set; } = string.Empty;
    public string ProductNumber { get; protected set; } = string.Empty;
    public long NumberSequence { get; protected set; }
    public string Currency { get; protected set; } = string.Empty;
    public Money Balance { get; protected set; } = default!;
    public ProductCategory Category { get; protected set; }
    public ProductStatus Status { get; protected set; }
    public DateTime CreatedAtUtc { get; protected set; }

    protected void EnsureActive(string errorMessage)
    {
        if (Status != ProductStatus.Active)
            throw new DomainException(errorMessage);
    }

    protected static void EnsurePositive(Money amount)
    {
        if (amount.Amount <= 0)
            throw new DomainException("Amount must be greater than zero.");
    }

    protected void SetBalance(Money balance)
        => Balance = balance;

    protected void RaiseCreatedDomainEvent(string productType)
    {
        AddDomainEvent(new ProductCreatedDomainEvent(
            Id,
            CustomerId.Value,
            Category.ToString(),
            productType,
            ProductName,
            ProductNumber,
            Balance.Amount,
            Currency));
    }
}
