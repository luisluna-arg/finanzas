using FinanceApi.Application.Dtos.Currency;

namespace FinanceApi.Application.Dtos.Module;

public record ModuleDto : Dto
{
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid? CurrencyId { get; set; }
    public CurrencyDto? Currency { get; set; }
}
