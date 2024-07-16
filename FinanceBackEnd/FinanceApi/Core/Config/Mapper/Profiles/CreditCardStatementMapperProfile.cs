using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class CreditCardStatementMapperProfile : BaseEntityMapperProfile<CreditCardStatement, CreditCardStatementDto>
{
    public CreditCardStatementMapperProfile()
        : base()
    {
    }
}
