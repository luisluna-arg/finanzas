using FinanceApi.Domain.Enums;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class AppModuleType : Entity<short>
{
    required public AppModuleTypeEnum Name { get; set; }
}
