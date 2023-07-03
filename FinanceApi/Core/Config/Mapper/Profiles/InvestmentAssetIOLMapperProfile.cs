using AutoMapper;
using FinanceApi.Application.Dtos.InvestmentAssetIOLs;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class InvestmentAssetIOLMapperProfile : Profile
{
    public InvestmentAssetIOLMapperProfile() => CreateMap<InvestmentAssetIOL, InvestmentAssetIOLDto>();
}
