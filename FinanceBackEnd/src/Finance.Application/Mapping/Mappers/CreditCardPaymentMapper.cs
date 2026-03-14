using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Mapping.Mappers;

public class CreditCardPaymentMapper : BaseMapper<CreditCardPayment, CreditCardPaymentDto>, ICreditCardPaymentMapper
{
    public CreditCardPaymentMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICreditCardPaymentMapper : IMapper<CreditCardPayment, CreditCardPaymentDto>;
