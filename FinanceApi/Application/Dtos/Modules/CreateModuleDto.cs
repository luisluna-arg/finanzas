namespace FinanceApi.Application.Dtos.Modules;

public abstract record CreateModuleDto
{
    protected CreateModuleDto()
    {
    }

    required public string Name { get; set; } = string.Empty;
    required public DateTime CreatedAt { get; set; }
    required public Guid CurrencyId { get; set; }
}
