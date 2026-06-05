using System.Security.Claims;
using Fortuna.Application.Abstractions.Security;
using Fortuna.Application.Common.Exceptions;

namespace Fortuna.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetRequiredCustomerId(this ClaimsPrincipal principal)
    {
        var claimValue = principal.FindFirstValue(CustomClaimNames.CustomerId);

        if (!Guid.TryParse(claimValue, out var customerId))
            throw new UnauthorizedException("Customer token is invalid.");

        return customerId;
    }
}
