using Finance.Application.Dtos.Debits;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class PaginatedDebitMapperProfile() : PaginatedResultMapperProfile<Debit, DebitDto>();