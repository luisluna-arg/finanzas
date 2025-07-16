namespace Finance.Application.Dtos.Base;

public abstract record CatalogDto<TId> : Dto<TId>
{
    public string Name { get; set; } = string.Empty;

    protected CatalogDto()
    {
    }
}