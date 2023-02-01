using FinanceApi.Helpers;

namespace FinanceApi.ApiMappings;
internal static class FundMappingExtensions
{
    private static string route = "/fund";
    internal static void FundMapping(this WebApplication app)
    {
        app.BaseMapping(route, (FinanceDb db) => db.Fund);

        app.MapPost(route + "/upload", async (IFormFileCollection files, FinanceDb db, DateTimeKind dateKind) =>
            {
                var module = db.Module.FirstOrDefault(o => o.Name == "Fondos");
                if (module == null) throw new Exception("Fund module not found");

                var excelHelper = new ExcelHelper();

                if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;
                var newRecords = excelHelper.ReadAsync(files, module, dateKind);

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
            });
    }

    internal static string CreateTempfilePath()
    {
        var filename = $"{Guid.NewGuid()}.tmp";
        var directoryPath = Path.Combine("temp", "uploads");
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        return Path.Combine(directoryPath, filename);
    }
}
