using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistance;

namespace Finance.Application.Repositories;

public class IdentityRepository(FinanceDbContext dbContext) : BaseRepository<Identity, Guid>(dbContext);
