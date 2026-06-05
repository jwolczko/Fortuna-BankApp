using Fortuna.Application.Abstractions.Messaging;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Application.Common.Exceptions;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Accounts.Repositories;

namespace Fortuna.Application.Accounts.Commands.WithdrawMoney;

public sealed class WithdrawMoneyCommandHandler : ICommandHandler<WithdrawMoneyCommand, Guid>
{
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawMoneyCommandHandler(IBankAccountRepository bankAccountRepository, IUnitOfWork unitOfWork)
    {
        _bankAccountRepository = bankAccountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(WithdrawMoneyCommand command, CancellationToken cancellationToken)
    {
        var account = await _bankAccountRepository.GetByIdAsync(command.AccountId, cancellationToken);
        if (account is null || account.CustomerId.Value != command.CustomerId)
            throw new NotFoundException("Bank account not found.");

        account.Withdraw(new Money(command.Amount, command.Currency), command.Title);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}
