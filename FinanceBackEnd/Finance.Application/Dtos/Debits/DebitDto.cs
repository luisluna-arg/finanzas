using Finance.Domain.SpecialTypes;

namespace Finance.Application.Dtos.Debits;

public record DebitDto : Dto<Guid>
{
    public DebitDto()
        : base()
    {
    }

    public Guid OriginId { get; set; }

    public string Origin { get; set; }

    public short AppModuleTypeId { get; set; }

    public string AppModuleType { get; set; }

    public Guid AppModuleId { get; set; }

    public string AppModule { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public Money Amount { get; set; }
}
