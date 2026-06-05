namespace Fortuna.Contracts.Accounts;

public sealed record DepositMoneyRequest(
    decimal Amount,
    string Currency,
    string Title);
