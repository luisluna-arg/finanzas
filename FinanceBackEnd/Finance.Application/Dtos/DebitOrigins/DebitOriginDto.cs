using Finance.Application.Dtos.AppModules;

namespace Finance.Application.Dtos.DebitOrigins;

public record DebitOriginDto : Dto<Guid>
{
    public DebitOriginDto()
        : base()
    {
    }

    public virtual AppModuleBasicDto AppModule { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    public int RecordCount { get; set; }
}
