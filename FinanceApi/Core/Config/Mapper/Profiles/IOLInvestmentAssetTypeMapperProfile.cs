using AutoMapper;
using FinanceApi.Application.Dtos.IOLInvestmentAssetTypes;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class IOLInvestmentAssetTypeMapperProfile : Profile
{
    public IOLInvestmentAssetTypeMapperProfile() => CreateMap<IOLInvestmentAssetType, IOLInvestmentAssetTypeDto>();
}
