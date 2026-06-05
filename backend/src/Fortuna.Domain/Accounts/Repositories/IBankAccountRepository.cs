namespace Fortuna.Domain.Accounts.Repositories;

public interface IBankAccountRepository
{
    Task AddAsync(BankAccount bankAccount, CancellationToken cancellationToken);
    Task<BankAccount?> GetByIdAsync(Guid bankAccountId, CancellationToken cancellationToken);
}
