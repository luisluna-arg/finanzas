using Finance.Application.Legacy.Dtos.Base;
using Finance.Domain.Enums;

namespace Finance.Application.Legacy.Dtos.Frequencies;

public record FrequencyDto : Dto<FrequencyEnum>
{
    public string Name { get; set; } = string.Empty;

    public FrequencyDto()
    {
    }
}
