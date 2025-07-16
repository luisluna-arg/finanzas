using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Commands.Funds;
using Finance.Application.Dtos.Funds;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/funds")]
public class FundCommandController(IMappingService mapper, IDispatcher dispatcher)
    : ApiBaseCommandController<Fund?, Guid, FundDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateFundCommand command)
        => await ExecuteAsync(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateFundCommand command)
        => await ExecuteAsync(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteFundsCommand request)
        => await ExecuteAsync(request);
}