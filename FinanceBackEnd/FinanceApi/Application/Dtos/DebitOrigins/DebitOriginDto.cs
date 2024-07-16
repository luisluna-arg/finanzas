using FinanceApi.Application.Dtos.AppModules;

namespace FinanceApi.Application.Dtos.DebitOrigins;

public record DebitOriginDto : Dto<Guid>
{
    public DebitOriginDto()
        : base()
    {
    }

    public virtual AppModuleBasicDto AppModule { get; set; }

    public string Name { get; set; }

    public int RecordCount { get; set; }
}
