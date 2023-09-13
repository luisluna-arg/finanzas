using System.ComponentModel;
using AutoMapper;
using FinanceApi.Application.Commands.DollarFunds;
using FinanceApi.Application.Commands.Funds;
using FinanceApi.Application.Dtos.Movements;
using FinanceApi.Application.Queries.Movements;
using FinanceApi.Domain.Models;
using FinanceApi.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/funds")]
public class FundController : ApiBaseController<Movement, Guid, MovementDto>
{
    public FundController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get(string? appModuleId)
        => await Handle(new GetAllFundMovementsQuery(appModuleId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => await Handle(new GetMovementQuery { Id = id });

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, Guid bankId, [DefaultValue("Local")] string dateKind)
    {
        await Handle(new UploadFundFileCommand(file, bankId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }

    [HttpPost]
    [Route("dollar-upload")]
    public async Task<IActionResult> DollarUpload(IFormFile file, Guid bankId, [DefaultValue("Local")] string dateKind)
    {
        await Handle(new UploadDollarFundsFileCommand(file, bankId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }

    [HttpPost]
    [Route("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFileCollection files, DateTime? dateReference, [DefaultValue("Local")] string? dateKind)
    {
        await Handle(new UploadImageCommand
        {
            Files = files,
            DateKind = ParsingHelper.ParseDateTimeKind(dateKind),
            DateReference = dateReference
        });
        return Ok();
    }

    [HttpPost]
    [Route("process-image")]
    public async Task<IActionResult> ProcessImage(HttpContext httpContext, IFormFileCollection files, [DefaultValue("Local")] string? dateKind)
    {
        await Handle(new ProcessImageCommand
        {
            Files = files,
            DateKind = ParsingHelper.ParseDateTimeKind(dateKind),
            HttpContext = httpContext,
        });
        return Ok();
    }

    [HttpPost]
    [Route("read-image")]
    public async Task<IActionResult> ProcessImageToText(HttpContext httpContext, IFormFileCollection files, [DefaultValue("Local")] string? dateKind)
    {
        await Handle(new ProcessImageToTextCommand
        {
            Files = files,
            DateKind = ParsingHelper.ParseDateTimeKind(dateKind),
            HttpContext = httpContext,
        });
        return Ok();
    }
}
