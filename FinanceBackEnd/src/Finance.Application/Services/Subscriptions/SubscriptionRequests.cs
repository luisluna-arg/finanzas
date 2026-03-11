using Finance.Domain.Enums;

namespace Finance.Application.Services.Subscriptions;

public sealed record CreateSubscriptionRequest(
    Guid CurrencyId,
    string Name,
    decimal Price,
    FrequencyEnum Frequency);

public sealed record UpdateSubscriptionRequest(
    Guid Id,
    Guid CurrencyId,
    string Name,
    decimal Price,
    FrequencyEnum Frequency);

public sealed record DeleteSubscriptionRequest(Guid[] Ids);