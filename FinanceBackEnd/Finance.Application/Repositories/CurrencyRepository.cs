using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class CurrencyRepository : BaseRepository<Currency, Guid>
{
    public CurrencyRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
