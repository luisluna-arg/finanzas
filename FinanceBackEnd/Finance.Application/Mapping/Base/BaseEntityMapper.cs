using Finance.Application.Dtos.Base;
using Finance.Application.Mapping;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Interfaces;

namespace Finance.Application.Mappers.Base;

public abstract class BaseEntityMapper<TEntity, TDto> : BaseMapper<TEntity, TDto>
    where TEntity : IEntity
    where TDto : class, IDto, new()
{
    protected BaseEntityMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IAppModuleTypeMapper<TEntity, TDto> : IMapper<TEntity, TDto>
{
}
