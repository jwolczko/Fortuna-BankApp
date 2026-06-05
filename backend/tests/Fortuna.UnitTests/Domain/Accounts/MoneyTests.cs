using FluentAssertions;
using Fortuna.Domain.Accounts;
using Xunit;

namespace Fortuna.UnitTests.Domain.Accounts;

public sealed class MoneyTests
{
    [Fact]
    public void AddShouldReturnNewMoneyWithSummedAmount()
    {
        var left = new Money(10m, "PLN");
        var right = new Money(5m, "PLN");

        var result = left.Add(right);

        result.Amount.Should().Be(15m);
        result.Currency.Should().Be("PLN");
    }
}
