using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories.Base;

namespace FinanceApi.Infrastructure.Repositotories;

public class BankRepository : BaseRepository<Bank, Guid>
{
    public BankRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
