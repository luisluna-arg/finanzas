using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class CreditCardMovementRepository : BaseRepository<CreditCardMovement, Guid>
{
    public CreditCardMovementRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
