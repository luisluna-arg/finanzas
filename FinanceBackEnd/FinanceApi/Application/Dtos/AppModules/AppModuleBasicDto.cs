namespace FinanceApi.Application.Dtos.AppModules;

public record AppModuleBasicDto : Dto<Guid>
{
    public AppModuleBasicDto()
        : base()
    {
    }

    public string Name { get; set; } = string.Empty;
}