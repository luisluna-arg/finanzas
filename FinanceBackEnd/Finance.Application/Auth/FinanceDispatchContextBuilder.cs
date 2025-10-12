using CQRSDispatch.Strategies;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Application.Auth;

public class FinanceDispatchContextBuilder : DispatchContextBuilderAsync<FinanceDispatchContext>
{
    public FinanceDispatchContextBuilder(IServiceProvider serviceProvider)
        : base()
    {
        ServiceProvider = serviceProvider;
    }

    protected IServiceProvider ServiceProvider { get; }

    public override async Task<FinanceDispatchContext> BuildAsync(HttpRequest? httpRequest)
    {
        var context = await base.BuildAsync(httpRequest);

        var dbContext = ServiceProvider.GetRequiredService<FinanceDbContext>();

        var userResult = await dbContext.User
            .AsNoTracking()
            .Include(u => u.Identities)
            .FirstOrDefaultAsync(u => u.Identities.Any(i => i.SourceId == context.UserIdClaim));

        if (userResult == null)
        {
            throw new InvalidOperationException("Context User Info is null. Ensure the user is authenticated and has a valid user ID claim.");
        }

        context.UserInfo = userResult;

        return context;
    }
}