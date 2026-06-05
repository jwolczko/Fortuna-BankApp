using Fortuna.Api.Security;
using Fortuna.Application.Accounts.Commands.OpenBankAccount;
using Fortuna.Application.Accounts.Commands.WithdrawMoney;
using Fortuna.Contracts.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fortuna.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/accounts")]
public sealed class AccountsController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Open(
        [FromBody] OpenBankAccountRequest request,
        [FromServices] OpenBankAccountCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var customerId = User.GetRequiredCustomerId();

        var accountId = await handler.Handle(
            new OpenBankAccountCommand(customerId, request.AccountNumber, request.AccountName, request.Currency, request.AccountType),
            cancellationToken);

        return Ok(accountId);
    }

    [HttpPost("{accountId:guid}/withdraw")]
    public async Task<ActionResult<Guid>> Withdraw(
        Guid accountId,
        [FromBody] WithdrawMoneyRequest request,
        [FromServices] WithdrawMoneyCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var customerId = User.GetRequiredCustomerId();

        var result = await handler.Handle(
            new WithdrawMoneyCommand(customerId, accountId, request.Amount, request.Currency, request.Title),
            cancellationToken);

        return Ok(result);
    }
}
