using Finance.Application.Legacy.Dtos.Frequencies;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Frequencies;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class FrequencyMapper : BaseMapper<Frequency, FrequencyDto>, IFrequencyMapper
{
    public FrequencyMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IFrequencyMapper : IMapper<Frequency, FrequencyDto>;
