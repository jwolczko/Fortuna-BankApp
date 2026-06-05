using Fortuna.Application.Abstractions.Messaging;

namespace Fortuna.Application.Accounts.Commands.DepositMoney;

public sealed record DepositMoneyCommand(
    Guid AccountId,
    decimal Amount,
    string Currency,
    string Title) : ICommand<Guid>;
