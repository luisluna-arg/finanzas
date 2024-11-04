using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class DebitOriginRepository : BaseRepository<DebitOrigin, Guid>
{
    public DebitOriginRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }
}
