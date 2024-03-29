using FinanceApi.Application.Dtos.Debits;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class DebitMapperProfile : BaseEntityMapperProfile<Debit, DebitDto>
{
    public DebitMapperProfile()
        : base()
    {
        Map.ForMember(o => o.OriginId, o => o.MapFrom(m => m.OriginId));
        Map.ForMember(o => o.Origin, o => o.MapFrom(m => m.Origin.Name));
        Map.ForMember(o => o.AppModuleTypeId, o => o.MapFrom(m => m.Origin.AppModule.Type.Id));
        Map.ForMember(o => o.AppModuleType, o => o.MapFrom(m => m.Origin.AppModule.Type.Name));
        Map.ForMember(o => o.AppModuleId, o => o.MapFrom(m => m.Origin.AppModule.Id));
        Map.ForMember(o => o.AppModule, o => o.MapFrom(m => m.Origin.AppModule.Name));
    }
}
