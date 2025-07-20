using Finance.Application.Mapping.Base;
using Finance.Domain.Models;
using Finance.Application.Dtos.Banks;

namespace Finance.Application.Mapping.Mappers;

public class BankMapper : BaseMapper<Bank, BankDto>, IBankMapper
{
    public BankMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IBankMapper : IMapper<Bank, BankDto>
{
}
