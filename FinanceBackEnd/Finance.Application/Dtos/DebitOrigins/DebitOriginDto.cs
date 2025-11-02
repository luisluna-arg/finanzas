using Finance.Application.Dtos.Base;
using Finance.Application.Dtos.AppModules;

namespace Finance.Application.Dtos.DebitOrigins;

public record DebitOriginDto : Dto<Guid>
{
    public DebitOriginDto() { }

    public virtual AppModuleCatalogDto AppModule { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public int RecordCount { get; set; } = 0;
}
