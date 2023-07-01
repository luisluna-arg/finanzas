namespace FinanceApi.Application.Dtos.Movements;

public record MovementDto : BaseMovementDto
{
    public MovementDto()
        : base()
    {
    }

    public Guid AppModuleId { get; set; }
    public Guid? CurrencyId { get; set; }
}
