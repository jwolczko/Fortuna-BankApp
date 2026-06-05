using Fortuna.Application.Abstractions.Messaging;

namespace Fortuna.Application.Transfers.Commands.CreateTransfer;

public sealed record CreateTransferCommand(
    string TransferType,
    Guid CustomerId,
    Guid SourceAccountId,
    Guid? TargetAccountId,
    string? TargetAccountNumber,
    string? RecipientName,
    decimal Amount,
    string Currency,
    string Title) : ICommand<Guid>;
