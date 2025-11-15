using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Dtos.DebitOrigins;
using Finance.Application.Mapping;
using Finance.Domain.Models.Debits;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/debit-origins")]
public class DebitOriginCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<DebitOrigin?, Guid, DebitOriginDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateDebitOriginCommand command)
        => await ExecuteAsync(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateDebitOriginCommand command)
        => await ExecuteAsync(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteDebitOriginCommand request)
        => await ExecuteAsync(request);
}
