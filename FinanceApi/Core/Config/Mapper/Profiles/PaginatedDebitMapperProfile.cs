using AutoMapper;
using FinanceApi.Application.Dtos.Debits;
using FinanceApi.Commons;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class PaginatedDebitMapperProfile : Profile
{
    public PaginatedDebitMapperProfile()
    {
        CreateMap<PaginatedResult<Debit>, PaginatedResult<DebitDto>>();
    }
}