using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Dtos.Subscriptions;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.Subscriptions;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/Subscriptions")]
public class SubscriptionCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    SubscriptionService subscriptionService)
    : CommandController<
        Subscription,
        SubscriptionPermissions,
        CreateSubscriptionRequest,
        UpdateSubscriptionRequest,
        DeleteSubscriptionRequest,
        SetSubscriptionOwnerRequest,
        DeleteSubscriptionOwnerRequest,
        Guid,
        SubscriptionDto,
        SubscriptionService>(mapper, dispatcher, subscriptionService)
{
}