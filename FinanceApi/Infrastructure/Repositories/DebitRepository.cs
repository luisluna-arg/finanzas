using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class DebitRepository : BaseRepository<Debit, Guid>
{
    public DebitRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
