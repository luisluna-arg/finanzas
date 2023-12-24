using FinanceApi.Application.Dtos.DebitOrigins;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class DebitOriginMapperProfile : BaseMapperProfile<DebitOrigin, DebitOriginDto>
{
    public DebitOriginMapperProfile()
        : base()
    {
    }
}
