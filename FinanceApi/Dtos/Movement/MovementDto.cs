using MovementEntity = FinanceApi.Models.Movement;

namespace FinanceApi.Dtos.Movement;

public class MovementDto : BaseMovementDto
{
    public Guid Id { get; set; }
    public Guid ModuleId { get; set; }
    public Guid? CurrencyId { get; set; }

    public override FinanceApi.Models.Movement BuildEntity()
    {
        var result = base.BuildEntity();
        result.Id = Id;
        result.ModuleId = ModuleId;
        result.CurrencyId = CurrencyId;
        return result;
    }
}
