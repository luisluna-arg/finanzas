using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Identities;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Base;

public abstract class ActivateDeactivateTestBase : IDisposable
{
    protected readonly FinanceDbContext DbContext;
    protected readonly User CurrentUser;

    protected ActivateDeactivateTestBase()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        DbContext = new FinanceDbContext(options, null);

        CurrentUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { Id = Guid.NewGuid(), SourceId = "IdentityNotFound" }],
        };

        DbContext.User.Add(CurrentUser);
        DbContext.SaveChanges();
    }

    public void Dispose() => DbContext.Dispose();
}
