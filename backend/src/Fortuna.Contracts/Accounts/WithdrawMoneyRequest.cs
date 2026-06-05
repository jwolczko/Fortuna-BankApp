namespace Fortuna.Contracts.Accounts;

public sealed record WithdrawMoneyRequest(
    decimal Amount,
    string Currency,
    string Title);
