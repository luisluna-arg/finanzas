using Finance.Application.Dtos.Base;
using Finance.Domain.Enums;

namespace Finance.Application.Dtos.Frequencies;

public record FrequencyDto : Dto<FrequencyEnum>
{
    public string Name { get; set; } = string.Empty;

    public FrequencyDto()
    {
    }
}
