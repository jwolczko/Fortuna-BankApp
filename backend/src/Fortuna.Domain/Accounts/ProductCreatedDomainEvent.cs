using Fortuna.Domain.Abstractions;

namespace Fortuna.Domain.Products.Events;

public sealed record ProductCreatedDomainEvent(
    Guid ProductId,
    Guid CustomerId,
    string ProductCategory,
    string ProductType,
    string ProductName,
    string ProductNumber,
    decimal Balance,
    string Currency) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
