using Finance.Application.Legacy.Dtos.Banks;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Banks;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class BankMapper : BaseMapper<Bank, BankDto>, IBankMapper
{
    public BankMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IBankMapper : IMapper<Bank, BankDto>;
