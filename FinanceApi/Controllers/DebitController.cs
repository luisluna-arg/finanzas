using System.ComponentModel;
using AutoMapper;
using FinanceApi.Application.Commands.Debits;
using FinanceApi.Application.Dtos.Debits;
using FinanceApi.Application.Queries.Debits;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/debits")]
public class DebitController : ApiBaseController<Debit?, Guid, DebitDto>
{
    public DebitController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllDebitsQuery request)
        => await Handle(request);

    [HttpPost]
    public async Task<IActionResult> Create(CreateDebitCommand command)
        => await Handle(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteDebitCommand request)
        => await Handle(request);

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string appModuleId, [DefaultValue("Local")] string dateKind)
    {
        await Handle(new UploadDebitsFileCommand(file, appModuleId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle(new ActivateDebitCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateDebitCommand { Id = id });

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedDebitsQuery request)
        => await Handle(request);
}
