using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.DebitOrigins;
using Finance.Application.Legacy.Dtos.DebitOrigins;
using Finance.Application.Legacy.Mapping;
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
