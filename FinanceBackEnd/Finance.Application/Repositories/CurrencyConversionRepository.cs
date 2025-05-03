using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class CurrencyConversionRepository : BaseRepository<CurrencyConversion, Guid>
{
    public CurrencyConversionRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
