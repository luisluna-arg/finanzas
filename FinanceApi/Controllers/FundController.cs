using AutoMapper;
using FinanceApi.Application.Commands.Funds;
using FinanceApi.Application.Dtos.Movements;
using FinanceApi.Application.Queries.Movements;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/funds")]
public class FundController : ApiBaseController<Movement, MovementDto>
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
    public async Task<IActionResult> Upload(IFormFileCollection files, DateTimeKind dateKind)
    {
        await Handle(new UploadFundFileCommand
        {
            Files = files,
            DateKind = dateKind
        });
        return Ok();
    }

    [HttpPost]
    [Route("api/funds/upload-image")]
    public async Task<IActionResult> UploadImage(IFormFileCollection files, DateTime? dateReference, DateTimeKind? dateKind)
    {
        await Handle(new UploadImageCommand
        {
            Files = files,
            DateKind = dateKind,
            DateReference = dateReference
        });
        return Ok();
    }

    [HttpPost]
    [Route("api/funds/process-image")]
    public async Task<IActionResult> ProcessImage(HttpContext httpContext, IFormFileCollection files, DateTimeKind? dateKind)
    {
        await Handle(new ProcessImageCommand
        {
            HttpContext = httpContext,
            Files = files,
            DateKind = dateKind
        });
        return Ok();
    }

    [HttpPost]
    [Route("api/funds/read-image")]
    public async Task<IActionResult> ProcessImageToText(HttpContext httpContext, IFormFileCollection files, DateTimeKind? dateKind)
    {
        await Handle(new ProcessImageToTextCommand
        {
            HttpContext = httpContext,
            Files = files,
            DateKind = dateKind
        });
        return Ok();
    }

    private static string CreateTempfilePath()
    {
        var filename = $"{Guid.NewGuid()}.tmp";
        var directoryPath = Path.Combine("temp", "uploads");
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        return Path.Combine(directoryPath, filename);
    }
}
