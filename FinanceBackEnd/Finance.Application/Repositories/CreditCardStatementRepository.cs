using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class CreditCardStatementRepository : BaseRepository<CreditCardStatement, Guid>
{
    public CreditCardStatementRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
