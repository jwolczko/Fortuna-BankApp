using Fortuna.Application.Dashboard.Queries.GetDashboard;
using Fortuna.Api.Security;
using Fortuna.Contracts.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fortuna.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    [HttpGet("{customerId:guid}")]
    public async Task<ActionResult<DashboardResponse>> Get(
        Guid customerId,
        [FromServices] GetDashboardQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var authenticatedCustomerId = User.GetRequiredCustomerId();
        if (authenticatedCustomerId != customerId)
            return Forbid();

        var dto = await handler.Handle(new GetDashboardQuery(customerId), cancellationToken);

        var response = new DashboardResponse(
            dto.CustomerId,
            dto.TotalBalance,
            dto.Currency,
            dto.Products.Select(x => new ProductTileResponse(
                x.ProductId,
                x.ProductCategory,
                x.ProductType,
                x.ProductName,
                x.ProductNumber,
                x.Balance,
                x.Currency)).ToList(),
            dto.Events.Select(x => new TimelineEventResponse(
                x.Id,
                x.EventDateUtc,
                x.EventType,
                x.Title,
                x.Amount,
                x.Currency,
                x.IsPositive)).ToList());

        return Ok(response);
    }
}
