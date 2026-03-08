using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Legacy.Commands.CreditCards;
using Finance.Application.Legacy.Dtos.CreditCards;
using Finance.Application.Legacy.Mapping;
using Finance.Domain.Models.CreditCards;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-card-statements")]
public class CreditCardStatementCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCUDCommandController<
    CreditCardStatement?,
    Guid,
    CreditCardStatementDto,
    CreateCreditCardStatementCommand,
    UpdateCreditCardStatementCommand,
    DeleteCreditCardStatementCommand
    >(mapper, dispatcher);
