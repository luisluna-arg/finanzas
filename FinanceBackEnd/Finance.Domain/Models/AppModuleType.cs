using Finance.Domain.Enums;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class AppModuleType : Entity<short>
{
    required public AppModuleTypeEnum Name { get; set; }
}
