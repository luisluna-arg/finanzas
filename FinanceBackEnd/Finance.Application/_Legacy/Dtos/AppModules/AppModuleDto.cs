using Finance.Application.Legacy.Dtos.Base;
using Finance.Application.Legacy.Dtos.Currencies;

namespace Finance.Application.Legacy.Dtos.AppModules;

public record AppModuleDto : Dto<Guid>
{
    public AppModuleDto() : base() { }

    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid? CurrencyId { get; set; }
    public CurrencyDto? Currency { get; set; }
    public AppModuleTypeDto? Type { get; set; }
}
