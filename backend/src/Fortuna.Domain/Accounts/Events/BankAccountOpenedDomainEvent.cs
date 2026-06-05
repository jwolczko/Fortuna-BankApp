using Fortuna.Domain.Abstractions;
using Fortuna.Domain.Customers;

namespace Fortuna.Domain.Accounts.Events;

[Obsolete("Use ProductCreatedDomainEvent instead.")]
public sealed record BankAccountOpenedDomainEvent(
    Guid AccountId,
    Guid CustomerId,
    string AccountNumber,
    string AccountName,
    decimal Balance,
    string Currency) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
