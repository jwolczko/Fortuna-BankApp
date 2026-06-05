namespace Fortuna.Domain.Accounts;

public sealed record BankAccountId(Guid Value)
{
    public static BankAccountId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
