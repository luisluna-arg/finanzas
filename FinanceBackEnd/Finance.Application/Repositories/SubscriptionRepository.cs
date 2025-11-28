using Finance.Application.Repositories.Base;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;

namespace Finance.Application.Repositories;

public class SubscriptionRepository(FinanceDbContext dbContext) : BaseRepository<Subscription, Guid>(dbContext);
