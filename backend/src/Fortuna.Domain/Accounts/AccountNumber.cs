using Fortuna.Domain.Abstractions;

namespace Fortuna.Domain.Accounts;

public sealed class AccountNumber : ValueObject
{
    public AccountNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Account number is required.");

        Value = value.Replace(" ", string.Empty).Trim();
    }

    public string Value { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
