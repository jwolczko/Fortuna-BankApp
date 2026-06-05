namespace Fortuna.ReadModel.Dashboard.ReadModels;

public sealed class TimelineEventReadModel
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? ProductId { get; set; }
    public DateTime EventDateUtc { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsPositive { get; set; }
}
