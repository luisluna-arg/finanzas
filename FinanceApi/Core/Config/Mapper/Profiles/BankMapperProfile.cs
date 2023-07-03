using AutoMapper;
using FinanceApi.Application.Dtos.Banks;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class BankMapperProfile : Profile
{
    public BankMapperProfile() => CreateMap<Bank, BankDto>();
}
