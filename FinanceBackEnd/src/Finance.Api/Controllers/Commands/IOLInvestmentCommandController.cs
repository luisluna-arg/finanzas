using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Dtos.IOLInvestments;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.IOLInvestments;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.IOLInvestments;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/iol-investment")]
public class IOLInvestmentCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    IOLInvestmentService iolInvestmentService)
    : CommandController<
        IOLInvestment,
        IOLInvestmentPermissions,
        CreateIOLInvestmentRequest,
        UpdateIOLInvestmentRequest,
        DeleteIOLInvestmentRequest,
        SetIOLInvestmentOwnerRequest,
        DeleteIOLInvestmentOwnerRequest,
        Guid,
        IOLInvestmentDto,
        IOLInvestmentService>(mapper, dispatcher, iolInvestmentService)
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        await Dispatcher.DispatchCommandAsync(new UploadIOLInvestmentsCommand(file));
        return Ok();
    }
}
