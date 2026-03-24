using Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Queries.Base;

public abstract class QueryHandlerBaseTests : IDisposable
{
    protected readonly FinanceDbContext _dbContext;

    protected QueryHandlerBaseTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);

        _dbContext.Database.EnsureCreated();
    }

    public virtual void Dispose() => _dbContext.Dispose();
}
