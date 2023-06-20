using AutoMapper;
using FinanceApi.Application.Dtos.Modules;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class ModuleMapperProfile : Profile
{
    public ModuleMapperProfile() => CreateMap<Module, ModuleDto>();
}
