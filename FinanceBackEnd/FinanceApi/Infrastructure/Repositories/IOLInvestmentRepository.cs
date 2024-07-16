using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class IOLInvestmentRepository : BaseRepository<IOLInvestment, Guid>
{
    public IOLInvestmentRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
