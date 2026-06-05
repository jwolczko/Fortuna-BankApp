using Fortuna.Domain.Accounts;
using Fortuna.Domain.Accounts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fortuna.Infrastructure.Persistence.Write.Repositories;

public sealed class BankAccountRepository : IBankAccountRepository
{
    private readonly WriteDbContext _dbContext;

    public BankAccountRepository(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        => _dbContext.BankAccounts.AddAsync(bankAccount, cancellationToken).AsTask();

    public Task<BankAccount?> GetByIdAsync(Guid bankAccountId, CancellationToken cancellationToken)
        => _dbContext.BankAccounts
            .Include(x => x.Transactions)
            .FirstOrDefaultAsync(x => x.Id == bankAccountId, cancellationToken);
}
