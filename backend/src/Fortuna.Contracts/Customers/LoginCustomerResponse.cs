namespace Fortuna.Contracts.Customers;

public sealed record LoginCustomerResponse(
    string Token,
    DateTime ExpiresAtUtc);
