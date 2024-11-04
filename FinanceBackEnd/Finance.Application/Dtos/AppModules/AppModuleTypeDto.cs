namespace Finance.Application.Dtos.AppModules;

public record AppModuleTypeDto : Dto<short>
{
    public AppModuleTypeDto()
        : base()
    {
    }

    public string Name { get; set; } = string.Empty;
}
