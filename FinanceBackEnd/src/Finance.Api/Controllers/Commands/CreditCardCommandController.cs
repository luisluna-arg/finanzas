using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.CreditCards;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.CreditCards;
using Finance.Domain.Models.CreditCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-cards")]
public class CreditCardCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    CreditCardService creditCardService)
    : ApiBaseCommandController<CreditCard?, Guid, CreditCardDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCreditCardRequest request)
    {
        var result = await creditCardService.Create(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok(MappingService.Map<CreditCardDto>(result.Data));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCreditCardRequest request)
    {
        var result = await creditCardService.Update(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok(MappingService.Map<CreditCardDto>(result.Data));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCreditCardRequest request)
    {
        var result = await creditCardService.Delete(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> SetOwner(SetCreditCardOwnerRequest request)
    {
        var result = await creditCardService.SetOwner(request.ResourceId, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> DeleteOwner(DeleteCreditCardOwnerRequest request)
    {
        var result = await creditCardService.DeleteOwner(request.ResourceId, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }
}
