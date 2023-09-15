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
public class DebitController : ApiBaseController<Debit, Guid, DebitDto>
{
    public DebitController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get(string? appModuleId)
        => await Handle(new GetAllDebitsQuery(appModuleId));

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string appModuleId, [DefaultValue("Local")] string dateKind)
    {
        await Handle(new UploadDebitsFileCommand(file, appModuleId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }
}
