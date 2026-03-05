using Finance.Application.Legacy.Dtos.AppModules;
using Finance.Application.Legacy.Dtos.Base;

namespace Finance.Application.Legacy.Dtos.DebitOrigins;

public record DebitOriginDto : Dto<Guid>
{
    public DebitOriginDto() { }

    public virtual AppModuleCatalogDto AppModule { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public int RecordCount { get; set; } = 0;
}
