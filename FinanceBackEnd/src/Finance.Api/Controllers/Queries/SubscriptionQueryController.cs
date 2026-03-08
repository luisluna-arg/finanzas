using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.Subscriptions;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Queries.Subscriptions;
using Finance.Domain.Models.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/subscriptions")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class SubscriptionQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : BasicQueryController<Subscription, Guid, SubscriptionDto, GetAllSubscriptionsQuery, GetSubscriptionQuery>(mapper, dispatcher)
{
    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedSubscriptionsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }
}
