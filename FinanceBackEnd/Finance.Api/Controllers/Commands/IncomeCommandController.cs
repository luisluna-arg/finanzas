using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.Incomes;
using Finance.Application.Dtos.Incomes;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/incomes")]
public class IncomeCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<Income?, Guid, IncomeDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeCommand command)
        => await ExecuteAsync(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateIncomeCommand command)
        => await ExecuteAsync(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteIncomesCommand request)
        => await ExecuteAsync(request);
}
