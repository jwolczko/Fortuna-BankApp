using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Fortuna.Application.Abstractions.Clock;
using Fortuna.Application.Abstractions.Security;
using Fortuna.Infrastructure.Auth;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Fortuna.UnitTests.Security;

public sealed class JwtTokenProviderTests
{
    [Fact]
    public void CreateShouldEmitTokenValidForFiveMinutes()
    {
        var clock = Substitute.For<IDateTimeProvider>();
        var now = new DateTime(2026, 4, 9, 10, 0, 0, DateTimeKind.Utc);
        clock.UtcNow.Returns(now);

        var options = Options.Create(new JwtOptions
        {
            Issuer = "Fortuna.Api",
            Audience = "Fortuna.Client",
            SigningKey = "SuperSecretJwtSigningKeyForFortunaChangeMe1234567890",
            ExpirationMinutes = 5
        });

        var sut = new JwtTokenProvider(options, clock);
        var customerId = Guid.NewGuid();

        var result = sut.Create(customerId, "jan@example.com");
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);

        result.ExpiresAtUtc.Should().Be(now.AddMinutes(5));
        token.Claims.Should().Contain(x => x.Type == CustomClaimNames.CustomerId && x.Value == customerId.ToString());
    }
}
