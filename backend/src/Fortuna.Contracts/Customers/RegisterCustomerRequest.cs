namespace Fortuna.Contracts.Customers;

public sealed record RegisterCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string CustomerType);
