using AutoMapper;
using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Application.Queries.CreditCards;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/credit-cards")]
public class CreditCardQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<CreditCard?, Guid, CreditCardDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardsQuery query)
        => await Handle(query);
}
