using System.Runtime.Versioning;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Helpers;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Commands.Funds;

[SupportedOSPlatform("windows")]
public class ProcessImageCommandHandler : BaseResponselessHandler<ProcessImageCommand>
{
    private readonly IOcrHelper _ocrHelper;

    public ProcessImageCommandHandler(
        FinanceDbContext db,
        IOcrHelper? ocrHelper = null)
        : base(db)
    {
        _ocrHelper = ocrHelper ?? new OcrHelper();
    }

    public override async Task<CommandResult> ExecuteAsync(ProcessImageCommand command, CancellationToken cancellationToken)
    {
        var file = command.Files[0];
        MemoryStream ms = new MemoryStream();
        file.OpenReadStream().CopyTo(ms);
        var stream = _ocrHelper.AdjustImage(ms);

        // Devolver la imagen procesada como descarga
        var response = command.HttpContext.Response;
        response.ContentType = "image/jpeg";
        response.Headers.Append("Content-Disposition", "attachment; filename=image.jpg");
        response.ContentLength = stream.Length;
        stream.Seek(0, SeekOrigin.Begin);
        await stream.CopyToAsync(response.Body);
        return CommandResult.Success();
    }
}

public class ProcessImageCommand : ICommand
{
    required public HttpContext HttpContext { get; set; }
    required public IFormFileCollection Files { get; set; }
    public DateTimeKind? DateKind { get; set; }
}
