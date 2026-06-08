using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Mapping.Mappers;

public class CreditCardStatementImportTemplateMapper
    : BaseMapper<CreditCardStatementImportTemplate, CreditCardStatementImportTemplateDto>,
      ICreditCardStatementImportTemplateMapper
{
    public CreditCardStatementImportTemplateMapper(IMappingService mappingService) : base(mappingService) { }
}

public interface ICreditCardStatementImportTemplateMapper
    : IMapper<CreditCardStatementImportTemplate, CreditCardStatementImportTemplateDto>;
