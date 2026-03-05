using Finance.Application.Legacy.Dtos.Debits;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class DebitMapper : BaseMapper<Debit, DebitDto>, IDebitMapper
{
    public DebitMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IDebitMapper : IMapper<Debit, DebitDto>;
