using FinanceApi.Application.Dtos.Movements;
using FinanceApi.Core.Config.Mapper.Profiles.Base;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class PaginatedMovementMapperProfile : PaginatedResultMapperProfile<Movement, MovementDto>
{
    public PaginatedMovementMapperProfile()
        : base()
    {
    }
}