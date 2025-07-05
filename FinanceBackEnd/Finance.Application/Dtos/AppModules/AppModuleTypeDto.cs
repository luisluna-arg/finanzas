namespace Finance.Application.Dtos.AppModules;

public record AppModuleTypeDto() : Dto<short>
{
    public string Name { get; set; } = string.Empty;
}
