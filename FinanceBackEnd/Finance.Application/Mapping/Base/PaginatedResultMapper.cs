using Finance.Application.Commons;
using Finance.Domain.Models.Interfaces;
using Finance.Application.Dtos.Base;

namespace Finance.Application.Mapping.Base;

public abstract class PaginatedResultMapper<TEntity, TDto>
    : BaseMapper<PaginatedResult<TEntity>, PaginatedResult<TDto>>
    where TEntity : IEntity
    where TDto : IDto, new()
{
    public PaginatedResultMapper(IMappingService mappingService) : base(mappingService)
    {
    }

    public override PaginatedResult<TDto> Map(PaginatedResult<TEntity> source)
    {
        if (source == null) return new();

        var items = MappingService.Map<ICollection<TDto>>(source.Items);

        return new PaginatedResult<TDto>(items, source.Page, source.PageSize, source.TotalItems);
    }
}

public interface IPaginatedResultMapper<TEntity, TDto> : IMapper<PaginatedResult<TEntity>, PaginatedResult<TDto>>
{
}