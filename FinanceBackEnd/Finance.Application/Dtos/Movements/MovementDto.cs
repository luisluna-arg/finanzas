namespace Finance.Application.Dtos.Movements;

public record MovementDto : BaseMovementDto
{
    public Guid AppModuleId { get; set; } = Guid.Empty;
    public Guid? CurrencyId { get; set; } = null;
    public Guid? BankId { get; set; } = null;

    public MovementDto()
    {
    }
}
