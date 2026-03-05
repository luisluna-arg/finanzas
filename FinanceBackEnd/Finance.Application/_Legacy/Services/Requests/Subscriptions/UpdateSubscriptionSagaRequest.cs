using Finance.Application.Legacy.Services.Requests.Subscriptions.Owners.Base;
using Finance.Domain.Enums;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Legacy.Services.Requests.Subscriptions;

public class UpdateSubscriptionSagaRequest(Guid subscriptionId, Guid currencyId, string name, Money price, FrequencyEnum frequency)
    : UpsertSubscriptionSagaRequestBase(currencyId, name, price, frequency)
{
    public Guid SubscriptionId { get; } = subscriptionId;
}
