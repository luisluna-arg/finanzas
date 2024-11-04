using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class CreditCardRepository : BaseRepository<CreditCard, Guid>
{
    public CreditCardRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
