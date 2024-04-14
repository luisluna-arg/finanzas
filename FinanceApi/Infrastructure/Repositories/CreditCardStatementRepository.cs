using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class CreditCardStatementRepository : BaseRepository<CreditCardStatement, Guid>
{
    public CreditCardStatementRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
