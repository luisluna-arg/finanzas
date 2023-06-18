using FinanceApi.Application.Dtos.Module;
using FinanceApi.Application.Models;

namespace FinanceApi.DtoFactory;

public interface IModuleDtoFactory
{
    ModuleDto[] Create(IEnumerable<Module> modules);
    ModuleDto Create(Module module);
}
