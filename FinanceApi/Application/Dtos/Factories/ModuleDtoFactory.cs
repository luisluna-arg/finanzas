using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Domain.Models;

namespace FinanceApi.Application.Dtos.Factories;

public class AppModuleDtoFactory : IAppModuleDtoFactory
{
    public AppModuleDto Create(AppModule appModule) => new AppModuleDto()
    {
        Id = appModule.Id,
        CreatedAt = appModule.CreatedAt,
        Name = appModule.Name
    };

    public AppModuleDto[] Create(IEnumerable<AppModule> appModules)
        => appModules.Select(o => Create(o)).ToArray();
}
