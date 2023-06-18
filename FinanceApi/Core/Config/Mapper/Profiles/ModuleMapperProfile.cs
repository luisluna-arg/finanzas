using AutoMapper;
using FinanceApi.Application.Dtos.Modules;
using FinanceApi.Application.Models;

namespace FinanceApi.Core.Config.Mapper
{
    public class ModuleMapperProfile : Profile
    {
        public ModuleMapperProfile() => CreateMap<Module, ModuleDto>();
    }
}
