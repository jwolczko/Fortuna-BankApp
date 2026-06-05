namespace Fortuna.Application.Dashboard.Queries.GetDashboard;

public sealed record ProductTileDto(
    Guid ProductId,
    string ProductCategory,
    string ProductType,
    string ProductName,
    string ProductNumber,
    decimal Balance,
    string Currency);

public sealed record TimelineEventDto(
    Guid Id,
    DateTime EventDateUtc,
    string EventType,
    string Title,
    decimal Amount,
    string Currency,
    bool IsPositive);

public sealed record DashboardDto(
    Guid CustomerId,
    decimal TotalBalance,
    string Currency,
    IReadOnlyCollection<ProductTileDto> Products,
    IReadOnlyCollection<TimelineEventDto> Events);
