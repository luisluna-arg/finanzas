using AutoMapper;
using FinanceApi.Application.Commands.CreditCards;
using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Application.Queries.CreditCards;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/credit-cards")]
public class CreditCardController : ApiBaseController<CreditCard?, Guid, CreditCardDto>
{
    public CreditCardController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardsQuery query)
        => await Handle(query);

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
