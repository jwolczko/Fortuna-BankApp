using FluentAssertions;
using Fortuna.Application.Abstractions.Security;
using Fortuna.Application.Common.Exceptions;
using Fortuna.Application.Customers.Commands.LoginCustomer;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Customers.Repositories;
using NSubstitute;
using Xunit;

namespace Fortuna.UnitTests.Customers;

public sealed class LoginCustomerCommandHandlerTests
{
    [Fact]
    public async Task HandleShouldReturnTokenForValidCredentials()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var tokenProvider = Substitute.For<ITokenProvider>();
        var customer = new Customer(
            CustomerId.New(),
            new FullName("Jan", "Kowalski"),
            new Email("jan@example.com"),
            "stored-hash");

        repository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        passwordHasher.Verify("Secret123!", "stored-hash").Returns(true);
        tokenProvider.Create(customer.Id.Value, customer.Email.Value)
            .Returns(new AuthenticationToken("jwt-token", DateTime.UtcNow.AddMinutes(5)));

        var sut = new LoginCustomerCommandHandler(repository, passwordHasher, tokenProvider);

        var result = await sut.Handle(new LoginCustomerCommand("jan@example.com", "Secret123!"), CancellationToken.None);

        result.Token.Should().Be("jwt-token");
    }

    [Fact]
    public async Task HandleShouldThrowWhenCredentialsAreInvalid()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var tokenProvider = Substitute.For<ITokenProvider>();
        var customer = new Customer(
            CustomerId.New(),
            new FullName("Jan", "Kowalski"),
            new Email("jan@example.com"),
            "stored-hash");

        repository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        passwordHasher.Verify("wrong", "stored-hash").Returns(false);

        var sut = new LoginCustomerCommandHandler(repository, passwordHasher, tokenProvider);

        var act = () => sut.Handle(new LoginCustomerCommand("jan@example.com", "wrong"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
