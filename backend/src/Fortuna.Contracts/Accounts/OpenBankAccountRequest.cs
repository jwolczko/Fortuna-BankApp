namespace Fortuna.Contracts.Accounts;

public sealed record OpenBankAccountRequest(
    string AccountNumber,
    string AccountName,
    string Currency,
    int AccountType);
