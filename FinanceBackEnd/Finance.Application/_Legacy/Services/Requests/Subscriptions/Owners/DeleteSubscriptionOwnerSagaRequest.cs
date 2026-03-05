using CQRSDispatch;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Application.Legacy.Services.Requests.Subscriptions.Owners.Base;

namespace Finance.Application.Legacy.Services.Requests.Subscriptions.Owners;

public class DeleteSubscriptionOwnerSagaRequest(Guid id)
    : BaseSubscriptionOwnerSagaRequest<CommandResult>(id), ISagaRequest;
