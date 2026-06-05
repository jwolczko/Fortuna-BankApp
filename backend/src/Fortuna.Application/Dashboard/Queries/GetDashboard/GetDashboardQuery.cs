using Fortuna.Application.Abstractions.Messaging;

namespace Fortuna.Application.Dashboard.Queries.GetDashboard;

public sealed record GetDashboardQuery(Guid CustomerId) : IQuery<DashboardDto>;
