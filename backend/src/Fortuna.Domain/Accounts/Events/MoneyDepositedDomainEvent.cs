using Fortuna.Domain.Abstractions;

namespace Fortuna.Domain.Accounts.Events;

public sealed record MoneyDepositedDomainEvent(
    Guid AccountId,
    Guid CustomerId,
    decimal Amount,
    string Currency,
    string Title) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
