using Finance.Application.Commands.Subscriptions;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Enums;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Services.Requests.Subscriptions.Owners.Base;

public abstract class UpsertSubscriptionSagaRequestBase : CreateSubscriptionCommand, ISagaRequest
{
    protected UpsertSubscriptionSagaRequestBase(Guid currencyId, string name, Money price, FrequencyEnum frequency)
        : base()
    {
        CurrencyId = currencyId;
        Name = name;
        Price = price;
        Frequency = frequency;
    }
}
