using AutoMapper;
using FinanceApi.Application.Dtos.Movements;
using FinanceApi.Commons;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class PaginatedMovementMapperProfile : Profile
{
    public PaginatedMovementMapperProfile()
    {
        CreateMap<PaginatedResult<Movement>, PaginatedResult<MovementDto>>();
    }
}