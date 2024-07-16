using FinanceApi.Application.Dtos;
using FinanceApi.Domain.Models.Interfaces;

namespace FinanceApi.Core.Config.Mapper.Profiles.Base;

public abstract class BaseEntityMapperProfile<TEntity, TDto> : BaseMapperProfile<TEntity, TDto>
    where TEntity : IEntity
    where TDto : IDto
{
    protected BaseEntityMapperProfile()
        : base()
    {
    }
}
