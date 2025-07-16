namespace Finance.Application.Dtos.AppModules;

public abstract record CreateAppModuleDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CurrencyId { get; set; } = Guid.Empty;

    protected CreateAppModuleDto()
    {
    }
}
