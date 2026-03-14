using Finance.Application.Dtos.Subscriptions;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Subscriptions;

namespace Finance.Application.Mapping.Mappers;

public class SubscriptionMapper : BaseMapper<Subscription, SubscriptionDto>, ISubscriptionMapper
{
    public SubscriptionMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ISubscriptionMapper : IMapper<Subscription, SubscriptionDto>;
