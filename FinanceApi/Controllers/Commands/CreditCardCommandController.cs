using AutoMapper;
using FinanceApi.Application.Commands.CreditCards;
using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

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
