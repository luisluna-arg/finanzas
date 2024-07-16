using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class CreditCardRepository : BaseRepository<CreditCard, Guid>
{
    public CreditCardRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
