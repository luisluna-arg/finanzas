using FinanceApi.Application.Models;
using FinanceApi.Application.Queries.Modules;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Commands.Modules;

public class GetModuleQueryHandler : IRequestHandler<GetModuleQuery, Module>
{
    private readonly FinanceDbContext dbContext;

    public GetModuleQueryHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Module> Handle(GetModuleQuery request, CancellationToken cancellationToken)
    {
        var module = await dbContext.Module.Include(o => o.Currency).FirstOrDefaultAsync(o => o.Id == request.Id);
        if (module == null) throw new Exception("Module not found");
        return await Task.FromResult(module);
    }
}
