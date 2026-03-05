using Finance.Application.Legacy.Dtos.CreditCards;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class CreditCardPaymentMapper : BaseMapper<CreditCardPayment, CreditCardPaymentDto>, ICreditCardPaymentMapper
{
    public CreditCardPaymentMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface ICreditCardPaymentMapper : IMapper<CreditCardPayment, CreditCardPaymentDto>;
