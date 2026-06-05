using Fortuna.Application.Abstractions.Messaging;
using Fortuna.Application.Abstractions.Security;
using Fortuna.Application.Common.Exceptions;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Customers.Repositories;

namespace Fortuna.Application.Customers.Commands.LoginCustomer;

public sealed class LoginCustomerCommandHandler : ICommandHandler<LoginCustomerCommand, AuthenticationToken>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public LoginCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider)
    {
        _customerRepository = customerRepository;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task<AuthenticationToken> Handle(LoginCustomerCommand command, CancellationToken cancellationToken)
    {
        var email = new Email(command.Email);
        var customer = await _customerRepository.GetByEmailAsync(email, cancellationToken);

        if (customer is null || !_passwordHasher.Verify(command.Password, customer.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        return _tokenProvider.Create(customer.Id.Value, customer.Email.Value);
    }
}
