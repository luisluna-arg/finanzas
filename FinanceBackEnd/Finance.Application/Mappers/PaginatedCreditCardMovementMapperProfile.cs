using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class PaginatedCreditCardMovementMapperProfile() : PaginatedResultMapperProfile<CreditCardMovement, CreditCardMovementDto>();