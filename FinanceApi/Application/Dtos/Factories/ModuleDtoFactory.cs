using FinanceApi.Application.Dtos.Modules;
using FinanceApi.Domain.Models;

namespace FinanceApi.Application.Dtos.Factories;

public class ModuleDtoFactory : IModuleDtoFactory
{
    public ModuleDto Create(Module module) => new ModuleDto()
    {
        Id = module.Id,
        CreatedAt = module.CreatedAt,
        Name = module.Name
    };

    public ModuleDto[] Create(IEnumerable<Module> modules)
        => modules.Select(o => Create(o)).ToArray();
}
