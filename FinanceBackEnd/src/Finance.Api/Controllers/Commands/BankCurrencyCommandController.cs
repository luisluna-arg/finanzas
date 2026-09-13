using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.BankCurrencies;
using Finance.Application.Dtos.BankCurrencies;
using Finance.Application.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/bank-currencies")]
public class BankCurrencyCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : SecuredApiController
{
    [HttpPut]
    public async Task<IActionResult> Upsert(UpsertBankCurrencyCommand command)
    {
        var result = await dispatcher.DispatchAsync(command, Request);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

        return Ok(mapper.Map<BankCurrencyDto>(result.Data!));
    }

    [HttpDelete("{bankId}/{currencyId}")]
    public async Task<IActionResult> Delete(Guid bankId, Guid currencyId)
    {
        var result = await dispatcher.DispatchCommandAsync(new DeleteBankCurrencyCommand { BankId = bankId, CurrencyId = currencyId });
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

        return Ok();
    }
}
