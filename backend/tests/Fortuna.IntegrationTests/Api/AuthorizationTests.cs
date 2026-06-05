using System.Net.Http.Json;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Fortuna.IntegrationTests.Api;

public sealed class AuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthorizationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AccountEndpointsShouldRequireBearerToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/accounts", new
        {
            accountNumber = "PL001234567890",
            accountName = "Main",
            currency = "PLN"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
