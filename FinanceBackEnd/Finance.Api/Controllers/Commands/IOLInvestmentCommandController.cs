using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Dtos.IOLInvestments;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/iol-investment")]
public class IOLInvestmentCommandController(IMappingService mapper, IDispatcher dispatcher)
    : ApiBaseCommandController<IOLInvestment?, Guid, IOLInvestmentDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateIOLInvestmentCommand command)
        => await ExecuteAsync(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteIOLInvestmentCommand request)
        => await ExecuteAsync(request);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateIOLInvestmentCommand command)
        => await ExecuteAsync(command);

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        await ExecuteAsync(new UploadIOLInvestmentsCommand(file));
        return Ok();
    }

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await ExecuteAsync(new ActivateIOLInvestmentCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await ExecuteAsync(new DeactivateIOLInvestmentCommand { Id = id });
}
