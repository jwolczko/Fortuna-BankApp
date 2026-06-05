using Fortuna.Application.Customers.Commands.LoginCustomer;
using Fortuna.Application.Customers.Commands.RegisterCustomer;
using Fortuna.Contracts.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fortuna.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<RegisterCustomerResponse>> Register(
        [FromBody] RegisterCustomerRequest request,
        [FromServices] RegisterCustomerCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var customerId = await handler.Handle(
            new RegisterCustomerCommand(request.FirstName, request.LastName, request.Email, request.Password, request.CustomerType),
            cancellationToken);

        return CreatedAtAction(nameof(Register), new RegisterCustomerResponse(customerId));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginCustomerResponse>> Login(
        [FromBody] LoginCustomerRequest request,
        [FromServices] LoginCustomerCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var token = await handler.Handle(
            new LoginCustomerCommand(request.Email, request.Password),
            cancellationToken);

        return Ok(new LoginCustomerResponse(token.Token, token.ExpiresAtUtc));
    }
}
