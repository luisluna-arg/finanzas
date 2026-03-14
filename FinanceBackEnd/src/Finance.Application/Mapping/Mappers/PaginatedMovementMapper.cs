using Finance.Application.Dtos.Movements;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.Movements;

namespace Finance.Application.Mapping.Mappers;

public class PaginatedMovementMapper : PaginatedResultMapper<Movement, MovementDto>, IPaginatedMovementMapper
{
    public PaginatedMovementMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedMovementMapper : IPaginatedResultMapper<Movement, MovementDto>;
