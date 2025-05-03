using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class DebitRepository : BaseRepository<Debit, Guid>
{
    public DebitRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
