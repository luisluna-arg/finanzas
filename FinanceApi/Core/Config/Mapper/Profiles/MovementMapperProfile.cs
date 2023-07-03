using AutoMapper;
using FinanceApi.Application.Dtos.Movements;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class MovementMapperProfile : Profile
{
    public MovementMapperProfile() => CreateMap<Movement, MovementDto>();
}
