using AutoMapper;
using FinanceApi.Application.Dtos.IOLInvestmentAssets;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class IOLInvestmentAssetMapperProfile : Profile
{
    public IOLInvestmentAssetMapperProfile() => CreateMap<IOLInvestmentAsset, IOLInvestmentAssetDto>();
}
