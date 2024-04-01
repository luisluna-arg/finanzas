using AutoMapper;
using FinanceApi.Application.Commands.IOLInvestments;
using FinanceApi.Application.Dtos.IOLInvestments;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/iol-investment")]
public class IOLInvestmentCommandController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<IOLInvestment?, Guid, IOLInvestmentDto>(mapper, mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateIOLInvestmentCommand command)
        => await Handle(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteIOLInvestmentCommand request)
        => await Handle(request);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateIOLInvestmentCommand command)
        => await Handle(command);

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        await Handle(new UploadIOLInvestmentsCommand(file));
        return Ok();
    }

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle(new ActivateIOLInvestmentCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateIOLInvestmentCommand { Id = id });
}
