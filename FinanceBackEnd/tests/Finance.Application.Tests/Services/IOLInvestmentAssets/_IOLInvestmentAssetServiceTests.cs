using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Services;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Services.IOLInvestmentAssets;

public partial class IOLInvestmentAssetServiceTests : IDisposable
{
    private readonly Mock<IDispatcher<FinanceDispatchContext>> _dispatcher;
    private readonly FinanceDbContext _dbContext;
    private readonly IOLInvestmentAssetService _sut;

    public IOLInvestmentAssetServiceTests()
    {
        _dispatcher = new Mock<IDispatcher<FinanceDispatchContext>>();

        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
        _sut = new IOLInvestmentAssetService(_dispatcher.Object, _dbContext);
    }

    public void Dispose() => _dbContext.Dispose();
}
