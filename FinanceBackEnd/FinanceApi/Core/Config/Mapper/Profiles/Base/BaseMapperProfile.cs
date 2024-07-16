using AutoMapper;

namespace FinanceApi.Core.Config.Mapper.Profiles.Base;

public abstract class BaseMapperProfile<TSource, TDestination> : Profile
{
    protected BaseMapperProfile()
    {
        Map = CreateMap<TSource, TDestination>();
    }

    protected IMappingExpression<TSource, TDestination> Map { get; }
}
