using FinanceApi.Application.Commands.Funds;
using FinanceApi.Domain;
using FinanceApi.Helpers;

namespace FinanceApi.Application.Handlers.Funds;

public class ProcessImageCommandHandler : BaseResponselessHandler<ProcessImageCommand>
{
    public ProcessImageCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task Handle(ProcessImageCommand command, CancellationToken cancellationToken)
    {
        var ocrHelper = new OcrHelper();

        var file = command.Files[0];
        MemoryStream ms = new MemoryStream();
        file.OpenReadStream().CopyTo(ms);
        var stream = ocrHelper.AdjustImage(ms);

        // Devolver la imagen procesada como descarga
        var response = command.HttpContext.Response;
        response.ContentType = "image/jpeg";
        response.Headers.Add("Content-Disposition", "attachment; filename=image.jpg");
        response.ContentLength = stream.Length;
        stream.Seek(0, SeekOrigin.Begin);
        await stream.CopyToAsync(response.Body);
    }
}
