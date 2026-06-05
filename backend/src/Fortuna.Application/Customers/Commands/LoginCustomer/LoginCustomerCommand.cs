using Fortuna.Application.Abstractions.Messaging;
using Fortuna.Application.Abstractions.Security;

namespace Fortuna.Application.Customers.Commands.LoginCustomer;

public sealed record LoginCustomerCommand(
    string Email,
    string Password) : ICommand<AuthenticationToken>;
