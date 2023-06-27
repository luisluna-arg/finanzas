using FinanceApi.Application.Queries.Modules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Modules;

public class GetModuleQueryHandler : IRequestHandler<GetModuleQuery, AppModule>
{
    private readonly FinanceDbContext dbContext;

    public GetModuleQueryHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<AppModule> Handle(GetModuleQuery request, CancellationToken cancellationToken)
    {
        var module = await dbContext.AppModule.Include(o => o.Currency).FirstOrDefaultAsync(o => o.Id == request.Id);
        if (module == null) throw new Exception("Module not found");
        return await Task.FromResult(module);
    }
}
