using Finance.Application.Dtos.Base;
using Finance.Domain.Enums;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Dtos.Debits;

public record DebitDto : Dto<Guid>
{
    public Guid OriginId { get; set; } = Guid.Empty;
    public string Origin { get; set; } = string.Empty;
    public AppModuleTypeEnum AppModuleTypeId { get; set; } = default;
    public string AppModuleType { get; set; } = string.Empty;
    public Guid AppModuleId { get; set; } = Guid.Empty;
    public string AppModule { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public Money Amount { get; set; } = default!;
    
    public DebitDto()
    {
    }
}
