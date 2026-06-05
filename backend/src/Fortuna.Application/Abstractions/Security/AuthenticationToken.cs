namespace Fortuna.Application.Abstractions.Security;

public sealed record AuthenticationToken(
    string Token,
    DateTime ExpiresAtUtc);
