using Finance.Application.Commands.Subscriptions;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services.Requests.Subscriptions;

public class DeleteSubscriptionSagaRequest : DeleteSubscriptionCommand, ISagaRequest;
