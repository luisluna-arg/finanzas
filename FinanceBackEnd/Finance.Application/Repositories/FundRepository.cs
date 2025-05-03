using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class FundRepository : BaseRepository<Fund, Guid>
{
    public FundRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
