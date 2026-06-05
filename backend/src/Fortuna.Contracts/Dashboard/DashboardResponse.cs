namespace Fortuna.Contracts.Dashboard;

public sealed record ProductTileResponse(
    Guid ProductId,
    string ProductCategory,
    string ProductType,
    string ProductName,
    string ProductNumber,
    decimal Balance,
    string Currency);

public sealed record TimelineEventResponse(
    Guid Id,
    DateTime EventDateUtc,
    string EventType,
    string Title,
    decimal Amount,
    string Currency,
    bool IsPositive);

public sealed record DashboardResponse(
    Guid CustomerId,
    decimal TotalBalance,
    string Currency,
    IReadOnlyCollection<ProductTileResponse> Products,
    IReadOnlyCollection<TimelineEventResponse> Events);
