using Finance.Application.Legacy.Repositories.Base;
using Finance.Domain.Models.Debits;
using Finance.Persistence;

namespace Finance.Application.Legacy.Repositories;

// Debit related repositories
public class DebitOriginRepository(FinanceDbContext dbContext) : BaseRepository<DebitOrigin, Guid>(dbContext);
