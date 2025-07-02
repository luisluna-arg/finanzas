using Finance.Application.Dtos.IOLInvestments;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class PaginatedIOLInvestmentMapperProfile() : PaginatedResultMapperProfile<IOLInvestment, IOLInvestmentDto>();