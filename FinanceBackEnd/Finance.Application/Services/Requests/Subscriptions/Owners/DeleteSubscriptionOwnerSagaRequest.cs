using CQRSDispatch;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.Requests.Subscriptions.Owners.Base;

namespace Finance.Application.Services.Requests.Subscriptions.Owners;

public class DeleteSubscriptionOwnerSagaRequest(Guid id)
    : BaseSubscriptionOwnerSagaRequest<CommandResult>(id), ISagaRequest;
