using Finance.Application.Legacy.Repositories.Base;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;

namespace Finance.Application.Legacy.Repositories;

public class SubscriptionRepository(FinanceDbContext dbContext) : BaseRepository<Subscription, Guid>(dbContext);
