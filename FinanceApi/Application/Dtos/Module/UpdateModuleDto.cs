namespace FinanceApi.Application.Dtos.Module;

public abstract class UpdateModuleDto
{
    public Guid Id { get; set; }
    required public string Name { get; set; } = string.Empty;
    required public DateTime CreatedAt { get; set; }
    required public Guid CurrencyId { get; set; }
}
