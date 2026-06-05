using Fortuna.Application.Transfers.Commands.CreateTransfer;
using Fortuna.Application.Accounts.Commands.DepositMoney;
using Fortuna.Api.Security;
using Fortuna.Contracts.Transfers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fortuna.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/transfers")]
public sealed class TransfersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateTransferRequest request,
        [FromServices] CreateTransferCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var customerId = User.GetRequiredCustomerId();

        var transferId = await handler.Handle(
            new CreateTransferCommand(
                request.TransferType,
                customerId,
                request.SourceAccountId,
                request.TargetAccountId,
                request.TargetAccountNumber,
                request.RecipientName,
                request.Amount,
                request.Currency,
                request.Title),
            cancellationToken);

        return Ok(transferId);
    }

    [HttpPost("incoming")]
    [AllowAnonymous]
    public async Task<ActionResult<Guid>> Incoming(
        [FromBody] IncomingTransferRequest request,
        [FromServices] DepositMoneyCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(
            new DepositMoneyCommand(
                request.TargetAccountId,
                request.Amount,
                request.Currency,
                request.Title),
            cancellationToken);

        return Ok(result);
    }
}
