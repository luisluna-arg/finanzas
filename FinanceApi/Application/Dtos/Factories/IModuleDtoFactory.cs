using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Domain.Models;

namespace FinanceApi.Application.Dtos.Factories;

public interface IAppModuleDtoFactory
{
    AppModuleDto[] Create(IEnumerable<AppModule> modules);
    AppModuleDto Create(AppModule module);
}
