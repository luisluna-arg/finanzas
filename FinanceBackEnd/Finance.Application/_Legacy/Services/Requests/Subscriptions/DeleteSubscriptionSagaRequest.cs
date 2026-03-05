using Finance.Application.Legacy.Commands.Subscriptions;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Services.Requests.Subscriptions;

public class DeleteSubscriptionSagaRequest : DeleteSubscriptionCommand, ISagaRequest;
