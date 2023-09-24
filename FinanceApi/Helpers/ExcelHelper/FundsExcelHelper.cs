using ExcelDataReader;
using FinanceApi.Domain.Models;

namespace FinanceApi.Helpers;

public class FundsExcelHelper : IFundsExcelHelper<Movement>
{
    public IEnumerable<Movement> Read(IEnumerable<IFormFile> files, AppModule appModule, Bank? bank, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        return files.SelectMany(o => Read(o, appModule, bank, dateTimeKind)).ToArray();
    }

    public IEnumerable<Movement> Read(IFormFile file, AppModule appModule, Bank? bank, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        List<Movement> records = new List<Movement>();

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
                for (var r = 3; r < sheet.Rows.Count; r++)
                {
                    records.Add(new Movement()
                    {
                        AppModuleId = appModule.Id,
                        AppModule = appModule,
                        Bank = bank,
                        TimeStamp = DateParser(sheet.Rows[r][0], dateTimeKind),
                        Concept1 = StringHelper.ValueOrEmpty(sheet.Rows[r][1]),
                        Concept2 = StringHelper.ValueOrEmpty(sheet.Rows[r][2]),
                        Amount = DecimalParser(sheet.Rows[r][3]),
                        Total = DecimalParser(sheet.Rows[r][4]),
                        Currency = appModule.Currency
                    });
                }
            }
        }

        return records.ToArray();
    }

    private DateTime DateParser(object? dateObject, DateTimeKind dateTimeKind)
    {
        if (dateObject != null)
        {
            var dateString = dateObject.ToString();
            if (!string.IsNullOrWhiteSpace(dateString))
            {
                var currentDate = DateTimeHelper.ParseDateTime(dateString, "dd/MM/yyyy", null, dateTimeKind);
                if (currentDate.ToShortDateString().Equals(DateTime.MinValue.ToShortDateString())) currentDate = DateTime.Now;
                return DateTimeHelper.FromTimeZoneToUTC(currentDate, -3);
            }
        }

        return DateTime.MinValue;
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
