using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class IncomeRepository : BaseRepository<Income, Guid>
{
    public IncomeRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
