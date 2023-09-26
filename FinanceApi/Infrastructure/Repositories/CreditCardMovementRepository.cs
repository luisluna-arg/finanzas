using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class CreditCardMovementRepository : BaseRepository<CreditCardMovement, Guid>
{
    public CreditCardMovementRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
