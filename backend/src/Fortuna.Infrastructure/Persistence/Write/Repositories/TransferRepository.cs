using Fortuna.Domain.Transfers;
using Fortuna.Domain.Transfers.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fortuna.Infrastructure.Persistence.Write.Repositories;

public sealed class TransferRepository : ITransferRepository
{
    private readonly WriteDbContext _dbContext;

    public TransferRepository(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Transfer transfer, CancellationToken cancellationToken)
        => _dbContext.Transfers.AddAsync(transfer, cancellationToken).AsTask();

    public Task<Transfer?> GetByIdAsync(TransferId transferId, CancellationToken cancellationToken)
        => _dbContext.Transfers.FirstOrDefaultAsync(x => x.Id == transferId, cancellationToken);
}
