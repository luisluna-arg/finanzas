using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class DebitOriginRepository(FinanceDbContext dbContext) : BaseRepository<DebitOrigin, Guid>(dbContext);
