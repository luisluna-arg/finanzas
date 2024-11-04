using Finance.Application.Dtos.Debits;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

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
