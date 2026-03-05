using Finance.Application.Legacy.Dtos.Movements;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Movements;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class PaginatedMovementMapper : PaginatedResultMapper<Movement, MovementDto>, IPaginatedMovementMapper
{
    public PaginatedMovementMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IPaginatedMovementMapper : IPaginatedResultMapper<Movement, MovementDto>;
