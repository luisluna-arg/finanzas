using Finance.Application.Legacy.Dtos.Subscriptions;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Subscriptions;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class SubscriptionMapper : BaseMapper<Subscription, SubscriptionDto>, ISubscriptionMapper
{
    public SubscriptionMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ISubscriptionMapper : IMapper<Subscription, SubscriptionDto>;
