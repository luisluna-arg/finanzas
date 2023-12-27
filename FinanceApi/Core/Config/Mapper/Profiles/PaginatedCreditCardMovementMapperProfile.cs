using AutoMapper;
using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Commons;
using FinanceApi.Domain.Models;

namespace FinanceApi.Core.Config.Mapper.Profiles;

public class PaginatedCreditCardMovementMapperProfile : Profile
{
    public PaginatedCreditCardMovementMapperProfile()
    {
        CreateMap<PaginatedResult<CreditCardMovement>, PaginatedResult<CreditCardMovementDto>>();
    }
}