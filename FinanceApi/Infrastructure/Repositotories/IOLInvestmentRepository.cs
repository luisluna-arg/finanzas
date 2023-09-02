using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories.Base;

namespace FinanceApi.Infrastructure.Repositotories;

public class IOLInvestmentRepository : BaseRepository<IOLInvestment, Guid>
{
    public IOLInvestmentRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
