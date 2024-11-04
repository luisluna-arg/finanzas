namespace Finance.Application.Dtos.AppModules;

public abstract record CreateAppModuleDto
{
    protected CreateAppModuleDto()
    {
    }

    required public string Name { get; set; } = string.Empty;
    required public DateTime CreatedAt { get; set; }
    required public Guid CurrencyId { get; set; }
}
