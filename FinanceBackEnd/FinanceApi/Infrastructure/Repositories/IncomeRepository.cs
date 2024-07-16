using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class IncomeRepository : BaseRepository<Income, Guid>
{
    public IncomeRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
