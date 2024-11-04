using Finance.Application.Dtos;
using Finance.Domain.Models.Interfaces;

namespace Finance.Application.Mappers.Base;

public abstract class BaseEntityMapperProfile<TEntity, TDto> : BaseMapperProfile<TEntity, TDto>
    where TEntity : IEntity
    where TDto : IDto
{
    protected BaseEntityMapperProfile()
        : base()
    {
    }
}
