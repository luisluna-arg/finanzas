namespace FinanceApi.Application.Dtos.AppModules;

public abstract record UpdateAppModuleDto : Dto<Guid>
{
    protected UpdateAppModuleDto()
        : base()
    {
    }

    required public string Name { get; set; } = string.Empty;

    required public DateTime CreatedAt { get; set; }

    required public Guid CurrencyId { get; set; }
}
