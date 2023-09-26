using System.ComponentModel;
using AutoMapper;
using FinanceApi.Application.Commands.CreditCards;
using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Application.Queries.CreditCardMovements;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/credit-card-movements")]
public class CreditCardMovementController : ApiBaseController<CreditCardMovement, Guid, CreditCardMovementDto>
{
    public CreditCardMovementController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardMovementsQuery query)
        => await Handle(query);

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string creditCardIssuerId, [DefaultValue("Local")] string dateKind)
    {
        await Handle(new UploadCreditCardFileCommand(file, creditCardIssuerId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }
}
