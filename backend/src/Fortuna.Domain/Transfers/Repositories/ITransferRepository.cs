namespace Fortuna.Domain.Transfers.Repositories;

public interface ITransferRepository
{
    Task AddAsync(Transfer transfer, CancellationToken cancellationToken);
    Task<Transfer?> GetByIdAsync(TransferId transferId, CancellationToken cancellationToken);
}
