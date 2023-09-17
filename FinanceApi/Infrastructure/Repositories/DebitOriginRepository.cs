using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;

namespace FinanceApi.Infrastructure.Repositories;

public class DebitOriginRepository : BaseRepository<DebitOrigin, Guid>
{
    public DebitOriginRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
