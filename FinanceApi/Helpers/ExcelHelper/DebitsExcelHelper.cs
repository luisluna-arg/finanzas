using ExcelDataReader;
using FinanceApi.Domain.Models;

namespace FinanceApi.Helpers.ExcelHelper;

public class DebitsExcelHelper : IAppModuleExcelHelper<Debit>
{
    public IEnumerable<Debit> ReadAsync(IEnumerable<IFormFile> files, AppModule appModule, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        return files.SelectMany(o => ReadAsync(o, appModule, dateTimeKind)).ToArray();
    }

    public IEnumerable<Debit> ReadAsync(IFormFile file, AppModule appModule, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        List<Debit> records = new List<Debit>();

        if (file == null || file.Length == 0)
            throw new Exception("File Not Selected");

        string fileExtension = Path.GetExtension(file.FileName);
        if (fileExtension != ".xls" && fileExtension != ".xlsx")
            throw new Exception("File Not Selected");

        using (var fileStream = file.OpenReadStream())
        {
            using (var reader = ExcelReaderFactory.CreateReader(fileStream))
            {
                var result = reader.AsDataSet();
                var sheet = result.Tables[0];

                var dateString = sheet.Rows[0][1].ToString();
                var currentDate = DateTimeHelper.ParseDateTime(dateString, "d/M/yyyy HH:mm:ss", null, dateTimeKind);
                if (currentDate.Ticks == DateTime.MinValue.Ticks) currentDate = DateTime.Now;
                currentDate = DateTimeHelper.FromTimeZoneToUTC(currentDate, -3);

                for (var r = 3; r < sheet.Rows.Count; r++)
                {
                    records.Add(new Debit()
                    {
                        DebitOrigin = new DebitOrigin()
                        {
                            AppModuleId = appModule.Id,
                            AppModule = appModule,
                            Name = StringHelper.ValueOrEmpty(sheet.Rows[r][0]).Trim()
                        },
                        TimeStamp = currentDate,
                        Amount = DecimalParser(sheet.Rows[r][1]),
                    });
                }
            }
        }

        return records.ToArray();
    }

    private decimal DecimalParser(object cell)
        => ParsingHelper.ParseDecimal(SanitizeDecimalString(cell));

    private string? SanitizeDecimalString(object value)
        => value?.ToString()?
            .Replace("$", string.Empty)
            .Replace("%", string.Empty)
            .Replace(".", string.Empty)
            .Replace(",", ".")
            .Trim();
}
