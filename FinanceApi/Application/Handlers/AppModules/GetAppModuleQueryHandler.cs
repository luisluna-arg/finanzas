using FinanceApi.Application.Queries.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.AppModules;

public class GetAppModuleQueryHandler : BaseResponseHandler<GetAppModuleQuery, AppModule>
{
    public GetAppModuleQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<AppModule> Handle(GetAppModuleQuery request, CancellationToken cancellationToken)
    {
        var appModule = await DbContext.AppModule.Include(o => o.Currency).FirstOrDefaultAsync(o => o.Id == request.Id);
        if (appModule == null) throw new Exception("App module not found");
        return await Task.FromResult(appModule);
    }
}
