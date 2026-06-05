using Fortuna.Domain.Abstractions;

namespace Fortuna.Domain.Transfers.Events;

public sealed record TransferCompletedDomainEvent(Guid TransferId) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
