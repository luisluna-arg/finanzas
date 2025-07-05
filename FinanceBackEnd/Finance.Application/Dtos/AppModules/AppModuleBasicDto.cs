namespace Finance.Application.Dtos.AppModules;

public record AppModuleBasicDto() : Dto<Guid>
{
    public string Name { get; set; } = string.Empty;
}