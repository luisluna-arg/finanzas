using Finance.Application.Legacy.Dtos.Debits;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class PaginatedDebitMapper : PaginatedResultMapper<Debit, DebitDto>, IPaginatedDebitMapper
{
    public PaginatedDebitMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedDebitMapper : IPaginatedResultMapper<Debit, DebitDto>;
