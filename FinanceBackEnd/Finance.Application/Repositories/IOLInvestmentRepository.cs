using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class IOLInvestmentRepository : BaseRepository<IOLInvestment, Guid>
{
    public IOLInvestmentRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
