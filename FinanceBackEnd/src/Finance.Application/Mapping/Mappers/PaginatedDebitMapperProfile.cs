using Finance.Application.Dtos.Debits;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Mapping.Mappers;

public class PaginatedDebitMapper : PaginatedResultMapper<Debit, DebitDto>, IPaginatedDebitMapper
{
    public PaginatedDebitMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedDebitMapper : IPaginatedResultMapper<Debit, DebitDto>;
