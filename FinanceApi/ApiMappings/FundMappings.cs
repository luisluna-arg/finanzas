using FinanceApi.Helpers;

namespace FinanceApi.ApiMappings;
internal static class FundMappingExtensions
{
    private static string route = "/fund";
    internal static void FundMapping(this WebApplication app)
    {
        app.BaseMapping(route, (FinanceDb db) => db.ModuleEntry);

        app.MapPost(route + "/upload", Upload);

        app.MapPost(route + "/upload-image", UploadImage);

        app.MapPost(route + "/process-image", ProcessImage);
        // {
        // System.Diagnostics.Debug.WriteLine("process-image");
        // IEnumerable<IFormFile> files = context.Request.Form.Files;
        // if (files != null && files.Count() > 0)
        // {
        //     var ocrHelper = new OCRHelperTesseract();

        //     // var file = files.;
        //     // MemoryStream ms = new MemoryStream();
        //     // file.OpenReadStream().CopyTo(ms);
        //     // var stream = ocrHelper.AdjustImage(ms);

        //     // // Crear un objeto de respuesta HTTP
        //     // var response = new Microsoft.AspNetCore.Http.HttpResponse();

        //     // return Results.Stream(stream, contentType: "image/jpeg");
        // }
        // });
    }

    internal static async void Upload(IFormFileCollection files, FinanceDb db, DateTimeKind dateKind)
    {
        var module = db.Module.FirstOrDefault(o => o.Name == "Fondos");
        if (module == null) throw new Exception("Fund module not found");

        var excelHelper = new ExcelHelper();

        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;
        var newRecords = excelHelper.ReadAsync(files, module, dateKind);

        if (newRecords == null || newRecords.Length == 0) return;

        var minDate = newRecords.Min(o => o.TimeStamp);
        var maxDate = newRecords.Max(o => o.TimeStamp);

        var existingRecords = db.Movement.Where(o => o.TimeStamp >= minDate && o.TimeStamp <= maxDate && o.Module.Id == module.Id);

        newRecords = newRecords.Where(o => existingRecords.All(x =>
            x.ModuleId != o.ModuleId ||
                x.TimeStamp != o.TimeStamp ||
                x.Ammount != o.Ammount ||
                x.Total != o.Total ||
                x.Concept1 != o.Concept1 ||
                x.Concept2 != o.Concept2)).ToArray();

        db.Movement.AddRange(newRecords);
        await db.SaveChangesAsync();
    }

    internal static async void UploadImage(IFormFileCollection files, FinanceDb db, DateTime? dateReference, DateTimeKind? dateKind)
    {
        var module = db.Module.FirstOrDefault(o => o.Name == "Fondos");
        if (module == null) throw new Exception("Fund module not found");

        var excelHelper = new OCRHelper();

        if (dateKind == null || dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;
        var (newMovements, newCurrencyConversions) = excelHelper.Process(files, db, module, dateReference ?? DateTime.Now, dateKind.Value);

        if (newMovements == null || newMovements.Length == 0) return;

        // var minDate = newRecords.Min(o => o.TimeStamp);
        // var maxDate = newRecords.Max(o => o.TimeStamp);

        // var existingRecords = db.Movement.Where(o => o.TimeStamp >= minDate && o.TimeStamp <= maxDate && o.Module.Id == module.Id);

        // newRecords = newRecords.Where(o => existingRecords.All(x =>
        //     x.ModuleId != o.ModuleId ||
        //         x.TimeStamp != o.TimeStamp ||
        //         x.Ammount != o.Ammount ||
        //         x.Total != o.Total ||
        //         x.Concept1 != o.Concept1 ||
        //         x.Concept2 != o.Concept2)).ToArray();

        // db.Movement.AddRange(newRecords);
        // await db.SaveChangesAsync();
    }

    internal static async void ProcessImage(HttpContext httpContext, IFormFileCollection files, FinanceDb db, DateTimeKind dateKind)
    {
        var ocrHelper = new OCRHelper();

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

    internal static string CreateTempfilePath()
    {
        var filename = $"{Guid.NewGuid()}.tmp";
        var directoryPath = Path.Combine("temp", "uploads");
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        return Path.Combine(directoryPath, filename);
    }
}
