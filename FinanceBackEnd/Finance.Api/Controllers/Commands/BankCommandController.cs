using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.Banks;
using Finance.Application.Legacy.Dtos.Banks;
using Finance.Application.Legacy.Mapping;
using Finance.Domain.Models.Banks;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/banks")]
public class BankCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<Bank?, Guid, BankDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateBankCommand command)
        => await ExecuteAsync(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateBankCommand command)
        => await ExecuteAsync(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteBankCommand request)
        => await ExecuteAsync(request);
}
