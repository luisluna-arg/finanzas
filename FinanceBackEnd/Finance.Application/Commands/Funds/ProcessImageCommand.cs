using System.Runtime.Versioning;
using Finance.Application.Base.Handlers;
using Finance.Helpers;
using Finance.Persistence;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Commands.Funds;

[SupportedOSPlatform("windows")]
public class ProcessImageCommandHandler : BaseResponselessHandler<ProcessImageCommand>
{
    public ProcessImageCommandHandler(
        FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<CommandResult> ExecuteAsync(ProcessImageCommand command, CancellationToken cancellationToken)
    {
        var ocrHelper = new OcrHelper();

        var file = command.Files[0];
        MemoryStream ms = new MemoryStream();
        file.OpenReadStream().CopyTo(ms);
        var stream = ocrHelper.AdjustImage(ms);

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
