using FinanceApi.Application.Dtos.Banks;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class BankMapperProfile : BaseEntityMapperProfile<Bank, BankDto>
{
    public BankMapperProfile()
        : base()
    {
    }
}
