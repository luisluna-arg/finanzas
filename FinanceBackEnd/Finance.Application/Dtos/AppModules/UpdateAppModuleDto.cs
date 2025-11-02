using Finance.Application.Dtos.Base;

namespace Finance.Application.Dtos.AppModules;

public abstract record UpdateAppModuleDto : Dto<Guid>
{
    protected UpdateAppModuleDto() { }

    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CurrencyId { get; set; } = Guid.Empty;
}
