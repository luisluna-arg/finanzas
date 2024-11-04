using Finance.Application.Dtos.Banks;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class BankMapperProfile : BaseEntityMapperProfile<Bank, BankDto>
{
    public BankMapperProfile()
        : base()
    {
    }
}
