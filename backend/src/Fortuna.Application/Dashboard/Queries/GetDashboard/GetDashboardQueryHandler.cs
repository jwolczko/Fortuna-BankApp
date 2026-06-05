using Fortuna.Application.Abstractions.Messaging;

namespace Fortuna.Application.Dashboard.Queries.GetDashboard;

public sealed class GetDashboardQueryHandler : IQueryHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IDashboardReadRepository _repository;

    public GetDashboardQueryHandler(IDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public Task<DashboardDto> Handle(GetDashboardQuery query, CancellationToken cancellationToken)
        => _repository.GetDashboardAsync(query.CustomerId, cancellationToken);
}
