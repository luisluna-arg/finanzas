using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class MovementRepository : BaseRepository<Movement, Guid>
{
    public MovementRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
