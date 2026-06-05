using Fortuna.Domain.Abstractions;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Transfers.Events;

namespace Fortuna.Domain.Transfers;

public sealed class Transfer : Entity<TransferId>, IAggregateRoot
{
    private Transfer()
    {
    }

    private Transfer(
        TransferId id,
        TransferType transferType,
        Guid sourceAccountId,
        Guid? targetAccountId,
        Money amount,
        string title,
        string? externalTargetAccountNumber,
        string? externalRecipientName) : base(id)
    {
        TransferType = transferType;
        SourceAccountId = sourceAccountId;
        TargetAccountId = targetAccountId;
        Amount = amount;
        Title = title;
        ExternalTargetAccountNumber = externalTargetAccountNumber;
        ExternalRecipientName = externalRecipientName;
        Status = TransferStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new TransferCreatedDomainEvent(
            id.Value,
            sourceAccountId,
            targetAccountId,
            amount.Amount,
            amount.Currency,
            title));
    }

    public TransferType TransferType { get; private set; }
    public Guid SourceAccountId { get; private set; }
    public Guid? TargetAccountId { get; private set; }
    public Money Amount { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string? ExternalTargetAccountNumber { get; private set; }
    public string? ExternalRecipientName { get; private set; }
    public TransferStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    public static Transfer CreateOwn(Guid sourceAccountId, Guid targetAccountId, Money amount, string title)
        => new(TransferId.New(), TransferType.Own, sourceAccountId, targetAccountId, amount, title, null, null);

    public static Transfer CreateExternal(
        Guid sourceAccountId,
        Money amount,
        string title,
        string externalTargetAccountNumber,
        string externalRecipientName)
        => new(
            TransferId.New(),
            TransferType.External,
            sourceAccountId,
            null,
            amount,
            title,
            externalTargetAccountNumber,
            externalRecipientName);

    public void MarkCompleted()
    {
        Status = TransferStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new TransferCompletedDomainEvent(Id.Value));
    }
}
