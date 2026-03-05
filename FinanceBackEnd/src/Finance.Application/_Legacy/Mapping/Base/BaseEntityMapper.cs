using Finance.Application.Legacy.Dtos.Base;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Interfaces;

namespace Finance.Application.Legacy.Mappers.Base;

public abstract class BaseEntityMapper<TEntity, TDto> : BaseMapper<TEntity, TDto>
    where TEntity : IEntity
    where TDto : class, IDto, new()
{
    protected BaseEntityMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IAppModuleTypeMapper<TEntity, TDto> : IMapper<TEntity, TDto>;
