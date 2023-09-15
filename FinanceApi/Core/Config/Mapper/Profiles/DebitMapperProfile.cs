using FinanceApi.Application.Dtos.Debits;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class DebitMapperProfile : BaseMapperProfile<Debit, DebitDto>
{
    public DebitMapperProfile()
        : base()
    {
    }
}
