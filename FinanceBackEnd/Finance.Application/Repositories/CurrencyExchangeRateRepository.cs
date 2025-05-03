using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class CurrencyExchangeRateRepository : BaseRepository<CurrencyExchangeRate, Guid>
{
    public CurrencyExchangeRateRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
