using AutoMapper;

namespace Finance.Application.Mappers.Base;

public abstract class BaseMapperProfile<TSource, TDestination> : Profile
{
    protected BaseMapperProfile()
    {
        Map = CreateMap<TSource, TDestination>();
    }

    protected IMappingExpression<TSource, TDestination> Map { get; }
}
