using AutoMapper;
using FinanceApi.Application.Dtos.IOLInvestments;
using FinanceApi.Commons;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class PaginatedIOLInvestmentMapperProfile : Profile
{
    public PaginatedIOLInvestmentMapperProfile()
    {
        CreateMap<PaginatedResult<IOLInvestment>, PaginatedResult<IOLInvestmentDto>>();
    }
}