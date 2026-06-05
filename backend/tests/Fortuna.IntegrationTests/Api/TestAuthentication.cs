using System.Net.Http.Headers;
using Fortuna.Application.Abstractions.Clock;
using Fortuna.Infrastructure.Auth;
using Microsoft.Extensions.Options;

namespace Fortuna.IntegrationTests.Api;

internal static class TestAuthentication
{
    public static void Authorize(HttpClient client, Guid customerId, string email = "test@example.com")
    {
        var tokenProvider = new JwtTokenProvider(
            Options.Create(new JwtOptions
            {
                Issuer = "Fortuna.Api",
                Audience = "Fortuna.Client",
                SigningKey = "SuperSecretJwtSigningKeyForFortunaChangeMe1234567890",
                ExpirationMinutes = 5
            }),
            new FixedDateTimeProvider());

        var token = tokenProvider.Create(customerId, email);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
    }

    private sealed class FixedDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
