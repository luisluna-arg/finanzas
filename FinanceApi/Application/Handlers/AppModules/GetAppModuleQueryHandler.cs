using FinanceApi.Application.Queries.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.AppModules;

public class GetAppModuleQueryHandler : IRequestHandler<GetAppModuleQuery, AppModule>
{
    private readonly FinanceDbContext dbContext;

    public GetAppModuleQueryHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<AppModule> Handle(GetAppModuleQuery request, CancellationToken cancellationToken)
    {
        var appModule = await dbContext.AppModule.Include(o => o.Currency).FirstOrDefaultAsync(o => o.Id == request.Id);
        if (appModule == null) throw new Exception("App module not found");
        return await Task.FromResult(appModule);
    }
}
