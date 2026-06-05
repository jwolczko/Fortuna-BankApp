using Fortuna.Application.Abstractions.Messaging;

namespace Fortuna.Application.Accounts.Commands.OpenBankAccount;

public sealed record OpenBankAccountCommand(
    Guid CustomerId,
    string AccountNumber,
    string AccountName,
    string Currency,
    int AccountType) : ICommand<Guid>;
