namespace FinanceApi.Application.Dtos.Debits;

public record DebitOriginDto : Dto<Guid>
{
    public DebitOriginDto()
        : base()
    {
    }

    public Guid AppModuleId { get; set; }

    required public string Name { get; set; }
}
