namespace Fortuna.Contracts.Transfers;

public sealed record CreateTransferRequest(
    string TransferType,
    Guid SourceAccountId,
    Guid? TargetAccountId,
    string? TargetAccountNumber,
    string? RecipientName,
    decimal Amount,
    string Currency,
    string Title);
