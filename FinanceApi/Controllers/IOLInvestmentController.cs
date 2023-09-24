using AutoMapper;
using FinanceApi.Application.Commands.IOLInvestments;
using FinanceApi.Application.Dtos.IOLInvestments;
using FinanceApi.Application.Queries.IOLInvestments;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/iol-investment")]
public class IOLInvestmentController : ApiBaseController<IOLInvestment?, Guid, IOLInvestmentDto>
{
    public IOLInvestmentController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetIOLInvestmentsQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetSingleIOLInvestmentQuery request)
        => await Handle(request);

    [HttpPost]
    public async Task<IActionResult> Create(CreateIOLInvestmentCommand command)
        => await Handle(command);

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
