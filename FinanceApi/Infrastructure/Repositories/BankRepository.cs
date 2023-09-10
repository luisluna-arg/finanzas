using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class BankRepository : BaseRepository<Bank, Guid>
{
    public BankRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
