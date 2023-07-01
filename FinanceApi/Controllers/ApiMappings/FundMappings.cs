using FinanceApi.Domain;
using FinanceApi.Helpers;

namespace FinanceApi.Controllers.ApiMappings;

internal static class FundMappingExtensions
{
    private static string route = "/fund";
    internal static void FundMapping(this WebApplication app)
    {
        app.BaseMapping(route, (FinanceDbContext db) => db.AppModuleEntry);

        app.MapPost(route + "/upload", Upload);

        app.MapPost(route + "/upload-image", UploadImage);

        app.MapPost(route + "/process-image", ProcessImage);

        app.MapPost(route + "/read-image", ProcessImageToText);
    }

    internal static async void Upload(IFormFileCollection files, FinanceDbContext db, DateTimeKind dateKind)
    {
        var appModule = db.AppModule.FirstOrDefault(o => o.Name == "Fondos");
        if (appModule == null) throw new Exception("Fund app module not found");

        var excelHelper = new ExcelHelper();

        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;
        var newRecords = excelHelper.ReadAsync(files, appModule, dateKind);

        if (newRecords == null || newRecords.Length == 0) return;

        var minDate = newRecords.Min(o => o.TimeStamp);
        var maxDate = newRecords.Max(o => o.TimeStamp);

        var existingRecords = db.Movement.Where(o => o.TimeStamp >= minDate && o.TimeStamp <= maxDate && o.AppModule.Id == appModule.Id);

        newRecords = newRecords.Where(o => existingRecords.All(x =>
            x.AppModuleId != o.AppModuleId ||
                x.TimeStamp != o.TimeStamp ||
                x.Amount != o.Amount ||
                x.Total != o.Total ||
                x.Concept1 != o.Concept1 ||
                x.Concept2 != o.Concept2)).ToArray();

        db.Movement.AddRange(newRecords);
        await db.SaveChangesAsync();
    }

    internal static void UploadImage(IFormFileCollection files, FinanceDbContext db, DateTime? dateReference, DateTimeKind? dateKind)
    {
        var appModule = db.AppModule.FirstOrDefault(o => o.Name == "Fondos");
        if (appModule == null) throw new Exception("Fund app module not found");

        // if (dateKind == null || dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;
        // var ocrHelper = new OcrHelper();
        // var (newMovements, newCurrencyConversions) = ocrHelper.Process(files, db, appModule, dateReference ?? DateTime.Now, dateKind.Value);
    }

    internal static async Task ProcessImage(HttpContext httpContext, IFormFileCollection files, FinanceDbContext db, DateTimeKind? dateKind)
    {
        var ocrHelper = new OcrHelper();

        var file = files[0];
        MemoryStream ms = new MemoryStream();
        file.OpenReadStream().CopyTo(ms);
        var stream = ocrHelper.AdjustImage(ms);

        // Devolver la imagen procesada como descarga
        var response = httpContext.Response;
        response.ContentType = "image/jpeg";
        response.Headers.Add("Content-Disposition", "attachment; filename=image.jpg");
        response.ContentLength = stream.Length;
        stream.Seek(0, SeekOrigin.Begin);
        await stream.CopyToAsync(response.Body);
    }

    internal static async Task ProcessImageToText(HttpContext httpContext, IFormFileCollection files, FinanceDbContext db, DateTimeKind? dateKind)
    {
        var ocrHelper = new OcrHelper();

        var textFromFiles = ocrHelper.CitricCaptureToImage(files);

        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8, -1, true);
        foreach (string textLine in textFromFiles)
        {
            writer.WriteLine(textLine);
        }

        await writer.FlushAsync();

        // Configurar respuesta HTTP
        httpContext.Response.ContentType = "text/plain";
        httpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"Lemon_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt\"");

        // Escribir contenido del MemoryStream en la respuesta HTTP
        stream.Seek(0, SeekOrigin.Begin);
        await stream.CopyToAsync(httpContext.Response.Body);
    }

    internal static string CreateTempfilePath()
    {
        var filename = $"{Guid.NewGuid()}.tmp";
        var directoryPath = Path.Combine("temp", "uploads");
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        return Path.Combine(directoryPath, filename);
    }
}
