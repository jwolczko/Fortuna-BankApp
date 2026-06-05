namespace Fortuna.Domain.Accounts;

public sealed record TransactionId(Guid Value)
{
    public static TransactionId New() => new(Guid.NewGuid());
}
