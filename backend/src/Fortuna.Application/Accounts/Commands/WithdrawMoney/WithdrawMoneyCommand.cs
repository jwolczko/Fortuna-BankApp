using Fortuna.Application.Abstractions.Messaging;

namespace Fortuna.Application.Accounts.Commands.WithdrawMoney;

public sealed record WithdrawMoneyCommand(
    Guid CustomerId,
    Guid AccountId,
    decimal Amount,
    string Currency,
    string Title) : ICommand<Guid>;
