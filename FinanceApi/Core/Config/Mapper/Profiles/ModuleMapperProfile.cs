using AutoMapper;
using FinanceApi.Application.Dtos.Module;
using FinanceApi.Application.Models;

namespace FinanceApi.Core.Config.Mapper
{
    public class ModuleMapperProfile : Profile
    {
        public ModuleMapperProfile() => CreateMap<Module, ModuleDto>();
    }
}
