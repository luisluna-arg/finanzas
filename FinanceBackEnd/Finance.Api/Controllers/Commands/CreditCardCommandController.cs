using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Dtos.CreditCards;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-cards")]
public class CreditCardController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<CreditCard?, Guid, CreditCardDto>(mapper, mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCreditCardCommand command)
    => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCreditCardCommand command)
        => await Handle(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCreditCardCommand request)
        => await Handle(request);
}
