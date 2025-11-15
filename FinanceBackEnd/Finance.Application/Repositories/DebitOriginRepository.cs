using Finance.Application.Repositories.Base;
using Finance.Domain.Models.Debits;
using Finance.Persistence;

namespace Finance.Application.Repositories;

// Debit related repositories
public class DebitOriginRepository(FinanceDbContext dbContext) : BaseRepository<DebitOrigin, Guid>(dbContext);
