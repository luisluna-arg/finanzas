using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Application.Queries.CreditCards;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/credit-cards")]
public class CreditCardQueryController(IMappingService mapper, IMediator mediator)
    : ApiBaseQueryController<CreditCard?, Guid, CreditCardDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardsQuery query)
        => await Handle(query);
}
