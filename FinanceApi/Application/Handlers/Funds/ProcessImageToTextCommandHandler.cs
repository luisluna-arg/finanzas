using FinanceApi.Application.Commands.Funds;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Helpers;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Funds;

public class ProcessImageToTextCommandHandler : BaseResponselessHandler<ProcessImageToTextCommand>
{
    public ProcessImageToTextCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository)
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
        command.HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"Lemon_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt\"");

        // Escribir contenido del MemoryStream en la respuesta HTTP
        stream.Seek(0, SeekOrigin.Begin);
        await stream.CopyToAsync(command.HttpContext.Response.Body);
    }
}
