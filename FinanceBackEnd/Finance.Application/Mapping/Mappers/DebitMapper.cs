using Finance.Application.Dtos.Debits;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models;

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
