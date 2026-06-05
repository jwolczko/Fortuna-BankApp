using Fortuna.Domain.Abstractions;

namespace Fortuna.Domain.Transfers.Events;

public sealed record TransferCreatedDomainEvent(
    Guid TransferId,
    Guid SourceAccountId,
    Guid? TargetAccountId,
    decimal Amount,
    string Currency,
    string Title) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
