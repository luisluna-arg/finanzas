using FinanceApi.Application.Dtos.Currencies;

namespace FinanceApi.Application.Dtos.Modules;

public record ModuleDto : Dto
{
    public ModuleDto()
        : base()
    {
    }

    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid? CurrencyId { get; set; }
    public CurrencyDto? Currency { get; set; }
}
