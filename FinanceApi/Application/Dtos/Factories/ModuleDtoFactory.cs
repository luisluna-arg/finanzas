using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Domain.Models;

namespace FinanceApi.Application.Dtos.Factories;

public class AppModuleDtoFactory : IAppModuleDtoFactory
{
    public AppModuleDto Create(AppModule module) => new AppModuleDto()
    {
        Id = module.Id,
        CreatedAt = module.CreatedAt,
        Name = module.Name
    };

    public AppModuleDto[] Create(IEnumerable<AppModule> modules)
        => modules.Select(o => Create(o)).ToArray();
}
