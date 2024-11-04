using Finance.Application.Dtos.Movements;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class PaginatedMovementMapperProfile : PaginatedResultMapperProfile<Movement, MovementDto>
{
    public PaginatedMovementMapperProfile()
        : base()
    {
    }
}