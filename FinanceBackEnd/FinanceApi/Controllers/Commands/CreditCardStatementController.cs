using AutoMapper;
using FinanceApi.Application.Commands.CreditCards;
using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/credit-card-statements")]
public class CreditCardStatementStatementController(IMapper mapper, IMediator mediator)
    : ApiBaseCUDCommandController<
    CreditCardStatement?,
    Guid,
    CreditCardStatementDto,
    CreateCreditCardStatementCommand,
    UpdateCreditCardStatementCommand,
    DeleteCreditCardStatementCommand
    >(mapper, mediator)
{
}
