using System.ComponentModel;
using AutoMapper;
using FinanceApi.Application.Commands.Funds;
using FinanceApi.Application.Dtos.Movements;
using FinanceApi.Application.Queries.Movements;
using FinanceApi.Domain.Models;
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
    public async Task<IActionResult> Get()
        => await Handle(new GetAllFundMovementsQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => await Handle(new GetMovementQuery { Id = id });

    [HttpPost]
    [Route("api/funds/upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] IFormFileCollection files, [DefaultValue("Local")] string dateKind)
    {
        await Handle(new UploadFundFileCommand
        {
            Files = files,
            DateKind = EnumHelper.Parse<DateTimeKind>(dateKind)
        });
        return Ok();
    }

    [HttpPost]
    [Route("api/funds/upload-image")]
    public async Task<IActionResult> UploadImage(IFormFileCollection files, DateTime? dateReference, [DefaultValue("Local")] string? dateKind)
    {
        await Handle(new UploadImageCommand
        {
            Files = files,
            DateKind = ParseDateTimeKind(dateKind),
            DateReference = dateReference
        });
        return Ok();
    }

    [HttpPost]
    [Route("api/funds/process-image")]
    public async Task<IActionResult> ProcessImage(HttpContext httpContext, IFormFileCollection files, [DefaultValue("Local")] string? dateKind)
    {
        await Handle(new ProcessImageCommand
        {
            Files = files,
            DateKind = ParseDateTimeKind(dateKind),
            HttpContext = httpContext,
        });
        return Ok();
    }

    [HttpPost]
    [Route("api/funds/read-image")]
    public async Task<IActionResult> ProcessImageToText(HttpContext httpContext, IFormFileCollection files, [DefaultValue("Local")] string? dateKind)
    {
        await Handle(new ProcessImageToTextCommand
        {
            Files = files,
            DateKind = ParseDateTimeKind(dateKind),
            HttpContext = httpContext,
        });
        return Ok();
    }

    private DateTimeKind ParseDateTimeKind(string? dateKind)
        => !string.IsNullOrWhiteSpace(dateKind) ? EnumHelper.Parse<DateTimeKind>(dateKind) : DateTimeKind.Local;
}
