using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Services;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Services.Subscriptions;

public partial class SubscriptionServiceTests : QueryHandlerBaseTests
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly SubscriptionService _sut;

    public SubscriptionServiceTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();
        _sut = new SubscriptionService(_dispatcher.Object, _dbContext);
    }
}