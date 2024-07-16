using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class CurrencyRepository : BaseRepository<Currency, Guid>
{
    public CurrencyRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
