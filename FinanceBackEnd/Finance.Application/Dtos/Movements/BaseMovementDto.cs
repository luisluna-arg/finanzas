using System.ComponentModel;
using Finance.Application.Dtos.Base;
using Finance.Domain.SpecialTypes;
using MovementEntity = Finance.Domain.Models.Movement;

namespace Finance.Application.Dtos.Movements;

public abstract record BaseMovementDto : Dto<Guid>
{
    protected BaseMovementDto()
        : base()
    {
        CreatedAt = DateTime.UtcNow;
        TimeStamp = DateTime.UtcNow;
    }

    public DateTime CreatedAt { get; set; }
    public DateTime TimeStamp { get; set; }
    [DefaultValue("Concept 1")]
    public string Concept1 { get; set; } = string.Empty;
    [DefaultValue("Concept 2")]
    public string? Concept2 { get; set; }
    [DefaultValue(5000)]
    public Money Amount { get; set; }
    [DefaultValue(100000)]
    public Money? Total { get; set; }

    public MovementEntity BuildEntity()
    {
        return new MovementEntity()
        {
            TimeStamp = TimeStamp,
            Concept1 = Concept1,
            Concept2 = Concept2,
            Amount = Amount,
            Total = Total
        };
    }
}
