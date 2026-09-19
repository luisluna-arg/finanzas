using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Domain.Models.CreditCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-card-installment-patterns")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class CreditCardInstallmentPatternCommandController(
    IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCUDCommandController<
        CreditCardInstallmentPattern,
        Guid,
        CreditCardInstallmentPatternDto,
        CreateCreditCardInstallmentPatternCommand,
        UpdateCreditCardInstallmentPatternCommand,
        DeleteCreditCardInstallmentPatternCommand>(mapper, dispatcher)
{
}
