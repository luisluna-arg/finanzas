using Finance.Domain.Enums;

namespace Finance.Application.Dtos.Frequencies;

public record FrequencyDto : Dto<FrequencyEnum>
{
    public FrequencyDto()
        : base()
    {
    }

    public string Name { get; set; }
}
