using System.ComponentModel;
using MovementEntity = FinanceApi.Models.Movement;

namespace FinanceApi.Dtos;

public abstract class BaseMovementDto : Dto<MovementEntity>
{
    protected BaseMovementDto()
        : base()
    {
        TimeStamp = DateTime.UtcNow;
    }

    public DateTime TimeStamp { get; set; }
    [DefaultValue("Concept 1")]
    public string Concept1 { get; set; } = string.Empty;
    [DefaultValue("Concept 2")]
    public string? Concept2 { get; set; }
    [DefaultValue(5000)]
    public decimal Amount { get; set; }
    [DefaultValue(100000)]
    public decimal? Total { get; set; }

    public override MovementEntity BuildEntity()
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
