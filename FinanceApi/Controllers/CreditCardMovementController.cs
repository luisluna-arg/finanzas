using System.ComponentModel;
using AutoMapper;
using FinanceApi.Application.Commands.CreditCards;
using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Application.Queries.CreditCards;
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
    public async Task<IActionResult> Get([FromQuery] GetCreditCardMovementsQuery request)
        => await Handle(request);

    [HttpPost]
    public async Task<IActionResult> Create(CreateCreditCardMovementCommand request)
        => await Handle(request);

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedCreditCardMovementsQuery request)
        => await Handle(request);

    [HttpGet]
    [Route("latest")]
    public async Task<IActionResult> Latest([FromQuery] GetLatestCreditCardMovementsQuery request)
        => await Handle(request);

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string creditCardId, [DefaultValue("Local")] string dateKind)
    {
        await Handle(new UploadCreditCardFileCommand(file, creditCardId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }
}
