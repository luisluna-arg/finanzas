using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Domain.Models;
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
