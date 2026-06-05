namespace Fortuna.Contracts.Customers;

public sealed record LoginCustomerRequest(
    string Email,
    string Password);
