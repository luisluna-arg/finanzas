using Finance.Application.Services.Requests.Subscriptions.Owners.Base;
using Finance.Domain.Enums;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Services.Requests.Subscriptions;

public class CreateSubscriptionSagaRequest(Guid currencyId, string name, Money price, FrequencyEnum frequency)
    : UpsertSubscriptionSagaRequestBase(currencyId, name, price, frequency);
