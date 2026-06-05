using Fortuna.Domain.Abstractions;
using Fortuna.Domain.Products;

namespace Fortuna.Domain.Customers;

public sealed class Customer : Entity<CustomerId>, IAggregateRoot
{
    private readonly List<Product> _products = [];

    private Customer()
    {
    }

    public Customer(CustomerId id, FullName fullName, Email email, string passwordHash)
        : this(id, fullName, email, passwordHash, CustomerType.Normal)
    {
    }

    public Customer(CustomerId id, FullName fullName, Email email, string passwordHash, CustomerType type) : base(id)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");

        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        Type = type;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public FullName FullName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = string.Empty;
    public CustomerType Type { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<Product> Products => _products;
}
