using FinanceApi.Models;
using MovementEntity = FinanceApi.Models.Movement;

namespace FinanceApi.Dtos;

public abstract class BaseMovementDto : Dto<MovementEntity>
{
    public DateTime TimeStamp { get; set; }
    public string Concept1 { get; set; } = string.Empty;
    public string? Concept2 { get; set; }
    public decimal Amount { get; set; }
    public decimal? Total { get; set; }

    public override MovementEntity BuildEntity()
    {
        return new MovementEntity()
        {
            TimeStamp = this.TimeStamp,
            Concept1 = this.Concept1,
            Concept2 = this.Concept2,
            Amount = this.Amount,
            Total = this.Total
        };
    }
}