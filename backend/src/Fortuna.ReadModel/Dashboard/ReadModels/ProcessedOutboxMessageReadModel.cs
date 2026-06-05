namespace Fortuna.ReadModel.Dashboard.ReadModels;

public sealed class ProcessedOutboxMessageReadModel
{
    public Guid OutboxMessageId { get; set; }
    public DateTime ProcessedAtUtc { get; set; }
}
