using Finance.Application.Mapping.Base;
using Finance.Domain.Models;
using Finance.Application.Dtos.Debits;

namespace Finance.Application.Mapping.Mappers;

public class DebitMapper : BaseMapper<Debit, DebitDto>, IDebitMapper
{
    public DebitMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IDebitMapper : IMapper<Debit, DebitDto>
{
}
