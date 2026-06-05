using Fortuna.Infrastructure.Persistence.Write;

namespace Fortuna.Infrastructure.Projections.Interfaces;

public interface IReadModelProjector
{
    Task ProjectAsync(OutboxMessage message, CancellationToken cancellationToken);
}
