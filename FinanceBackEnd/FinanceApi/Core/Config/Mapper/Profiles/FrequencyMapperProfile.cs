using FinanceApi.Application.Dtos.Frequencies;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class FrequencyMapperProfile : BaseEntityMapperProfile<Frequency, FrequencyDto>
{
    public FrequencyMapperProfile()
        : base()
    {
    }
}
