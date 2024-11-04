using Finance.Application.Dtos;
using Finance.Application.Commons;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models.Interfaces;

namespace Finance.Application.Mappers.Base;

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