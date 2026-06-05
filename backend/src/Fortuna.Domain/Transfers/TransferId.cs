namespace Fortuna.Domain.Transfers;

public sealed record TransferId(Guid Value)
{
    public static TransferId New() => new(Guid.NewGuid());
}
