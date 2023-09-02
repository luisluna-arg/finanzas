using AutoMapper;
using FinanceApi.Application.Dtos.IOLInvestments;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class IOLInvestmentMapperProfile : Profile
{
    public IOLInvestmentMapperProfile() => CreateMap<IOLInvestment, IOLInvestmentDto>();
}
