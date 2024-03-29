using FinanceApi.Application.Dtos;
using FinanceApi.Commons;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models.Interfaces;

namespace FinanceApi.Core.Config.Mapper.Profiles.Base;

public class PaginatedResultMapperProfile<TEntity, TDto>
    : BaseMapperProfile<PaginatedResult<TEntity>, PaginatedResult<TDto>>
    where TEntity : IEntity
    where TDto : IDto
{
    public PaginatedResultMapperProfile()
        : base()
    {
    }
}