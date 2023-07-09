using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories.Base;

namespace FinanceApi.Infrastructure.Repositotories;

public class CurrencyRepository : BaseRepository<Currency, Guid>
{
    public CurrencyRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
