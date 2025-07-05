using System.ComponentModel;
using Finance.Api.Controllers.Base;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Helpers;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-card-movements")]
public class CreditCardMovementCommandController(IMappingService mapper, IMediator mediator)
    : ApiBaseCommandController<CreditCardMovement, Guid, CreditCardMovementDto>(mapper, mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCreditCardMovementCommand request)
        => await Handle(request);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCreditCardMovementCommand request)
        => await Handle(request);

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string creditCardId, [DefaultValue("Local")] string dateKind)
    {
        await Handle(new UploadCreditCardFileCommand(file, creditCardId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }
}
