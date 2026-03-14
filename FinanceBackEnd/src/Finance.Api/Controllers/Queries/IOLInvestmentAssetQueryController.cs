using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Mapping;
using Finance.Application.Queries.IOLInvestmentAssets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/iol-investment-asset")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class IOLInvestmentAssetQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : SecuredApiController
{
    protected IMappingService MappingService { get => mapper; }
    protected IDispatcher<FinanceDispatchContext> Dispatcher { get => dispatcher; }

    [HttpGet]
    public async Task<IActionResult> GetTypes()
    {
        var result = await Dispatcher.DispatchQueryAsync(new GetAllIOLInvestmentAssetsQuery());
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeById(Guid id)
    {
        var result = await Dispatcher.DispatchQueryAsync(new GetIOLInvestmentAssetQuery { Id = id });
        return Ok(result.Data);
    }
}
