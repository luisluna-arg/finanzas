using Finance.Domain.Models;
using Finance.Persistance;
using Finance.Application.Repositories.Base;

namespace Finance.Application.Repositories;

public class BankRepository : BaseRepository<Bank, Guid>
{
    public BankRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
