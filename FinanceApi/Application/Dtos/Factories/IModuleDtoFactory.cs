using FinanceApi.Application.Dtos.Modules;
using FinanceApi.Domain.Models;

namespace FinanceApi.Application.Dtos.Factories;

public interface IModuleDtoFactory
{
    ModuleDto[] Create(IEnumerable<Module> modules);
    ModuleDto Create(Module module);
}
