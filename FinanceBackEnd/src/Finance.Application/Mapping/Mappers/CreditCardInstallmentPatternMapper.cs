using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Mapping.Mappers;

public class CreditCardInstallmentPatternMapper
    : BaseMapper<CreditCardInstallmentPattern, CreditCardInstallmentPatternDto>,
      ICreditCardInstallmentPatternMapper
{
    public CreditCardInstallmentPatternMapper(IMappingService mappingService) : base(mappingService) { }
}

public interface ICreditCardInstallmentPatternMapper
    : IMapper<CreditCardInstallmentPattern, CreditCardInstallmentPatternDto>;
