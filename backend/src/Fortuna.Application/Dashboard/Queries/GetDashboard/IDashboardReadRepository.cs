namespace Fortuna.Application.Dashboard.Queries.GetDashboard;

public interface IDashboardReadRepository
{
    Task<DashboardDto> GetDashboardAsync(Guid customerId, CancellationToken cancellationToken);
}
