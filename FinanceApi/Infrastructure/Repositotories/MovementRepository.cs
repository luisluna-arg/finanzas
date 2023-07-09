using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories.Base;

namespace FinanceApi.Infrastructure.Repositotories;

public class MovementRepository : BaseRepository<Movement, Guid>
{
    public MovementRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
