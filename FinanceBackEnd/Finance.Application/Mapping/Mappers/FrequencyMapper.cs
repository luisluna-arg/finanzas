using Finance.Application.Mapping.Base;
using Finance.Domain.Models;
using Finance.Application.Dtos.Frequencies;

namespace Finance.Application.Mapping.Mappers;

public class FrequencyMapper : BaseMapper<Frequency, FrequencyDto>, IFrequencyMapper
{
    public FrequencyMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IFrequencyMapper : IMapper<Frequency, FrequencyDto>
{
}
