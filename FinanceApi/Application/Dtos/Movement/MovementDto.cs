namespace FinanceApi.Application.Dtos.Movement;

public record MovementDto : BaseMovementDto
{
    public MovementDto() : base()
    {
    }

    public Guid ModuleId { get; set; }
    public Guid? CurrencyId { get; set; }
}
