using System.Runtime.Versioning;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Helpers;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Commands.Funds;

[SupportedOSPlatform("windows")]
public class ProcessImageToTextCommandHandler : BaseResponselessHandler<ProcessImageToTextCommand>
{
    private readonly IOcrHelper _ocrHelper;

    public ProcessImageToTextCommandHandler(FinanceDbContext db, IOcrHelper? ocrHelper = null)
        : base(db)
    {
        _ocrHelper = ocrHelper ?? new OcrHelper();
    }

    public override async Task<CommandResult> ExecuteAsync(ProcessImageToTextCommand command, CancellationToken cancellationToken)
    {
        var textFromFiles = _ocrHelper.CitricCaptureToImage(command.Files);

        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8, -1, true);
        foreach (string textLine in textFromFiles)
        {
            writer.WriteLine(textLine);
        }

        await writer.FlushAsync();

        // Configurar respuesta HTTP
        command.HttpContext.Response.ContentType = "text/plain";
        command.HttpContext.Response.Headers.Append("Content-Disposition", $"attachment; filename=\"Lemon_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt\"");

        // Escribir contenido del MemoryStream en la respuesta HTTP
        stream.Seek(0, SeekOrigin.Begin);
        await stream.CopyToAsync(command.HttpContext.Response.Body);
        return CommandResult.Success();
    }
}

public class ProcessImageToTextCommand : ICommand
{
    required public HttpContext HttpContext { get; set; }
    required public IFormFileCollection Files { get; set; }
    public DateTimeKind? DateKind { get; set; }
    public DateTime? DateReference { get; internal set; }
}
