using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Application.Queries.CreditCards;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/credit-cards")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class CreditCardQueryController(IMappingService mapper, IDispatcher dispatcher)
    : ApiBaseQueryController<CreditCard, Guid, CreditCardDto>(mapper, dispatcher)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardsQuery query)
        => await ExecuteAsync(query);
}
