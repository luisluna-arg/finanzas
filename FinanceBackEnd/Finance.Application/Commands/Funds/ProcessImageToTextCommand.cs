using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Helpers;
using Finance.Persistance;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Commands.Funds;

public class ProcessImageToTextCommandHandler : BaseResponselessHandler<ProcessImageToTextCommand>
{
    public ProcessImageToTextCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task Handle(ProcessImageToTextCommand command, CancellationToken cancellationToken)
    {
        var ocrHelper = new OcrHelper();

        var textFromFiles = ocrHelper.CitricCaptureToImage(command.Files);

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
    }
}

public class ProcessImageToTextCommand : IRequest
{
    required public HttpContext HttpContext { get; set; }

    required public IFormFileCollection Files { get; set; }

    public DateTimeKind? DateKind { get; set; }

    public DateTime? DateReference { get; internal set; }
}
