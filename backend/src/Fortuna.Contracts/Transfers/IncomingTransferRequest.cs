namespace Fortuna.Contracts.Transfers;

public sealed record IncomingTransferRequest(
    Guid TargetAccountId,
    decimal Amount,
    string Currency,
    string Title);
