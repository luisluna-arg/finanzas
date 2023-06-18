using FinanceApi.Application.Dtos.Module;
using FinanceApi.Application.Models;

namespace FinanceApi.DtoFactory;

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
