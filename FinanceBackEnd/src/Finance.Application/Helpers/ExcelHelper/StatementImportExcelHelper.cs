using ExcelDataReader;
using Microsoft.AspNetCore.Http;

namespace Finance.Helpers.ExcelHelper;

public class StatementImportExcelHelper
{
    public IEnumerable<StatementImportRow> Read(IFormFile file, StatementImportConfig config)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File not selected");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xls" && extension != ".xlsx" && extension != ".csv")
            throw new Exception("Unsupported file format. Use .xls, .xlsx, or .csv");

        using var stream = file.OpenReadStream();
        using var reader = extension == ".csv"
            ? ExcelReaderFactory.CreateCsvReader(stream)
            : ExcelReaderFactory.CreateReader(stream);

        var dataset = reader.AsDataSet();
        var sheet = dataset.Tables[0];
        var rows = new List<StatementImportRow>();

        var lastRow = sheet.Rows.Count - config.SkipLastRows;
        for (int r = config.SkipRows; r < lastRow; r++)
        {
            var row = sheet.Rows[r];
            var dateRaw = row[config.DateColumn]?.ToString();
            var conceptRaw = row[config.ConceptColumn]?.ToString();
            var amountRaw = row[config.AmountColumn]?.ToString();

            if (string.IsNullOrWhiteSpace(dateRaw) && string.IsNullOrWhiteSpace(conceptRaw))
                continue;

            var date = ParseDate(dateRaw, config.DateFormat);
            var amount = ParseDecimal(amountRaw, config.DecimalSeparator, config.ThousandsSeparator);
            if (config.AmountNegate) amount = -amount;

            rows.Add(new StatementImportRow
            {
                Date = date,
                Concept = conceptRaw ?? string.Empty,
                Amount = amount,
            });
        }

        return rows;
    }

    private static DateTime ParseDate(string? raw, string format)
    {
        if (string.IsNullOrWhiteSpace(raw)) return DateTime.UtcNow;

        if (DateTime.TryParseExact(raw, format,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var parsed))
            return DateTime.SpecifyKind(parsed, DateTimeKind.Utc);

        // Fallback: try common formats
        if (DateTime.TryParse(raw, out var fallback))
            return DateTime.SpecifyKind(fallback, DateTimeKind.Utc);

        return DateTime.UtcNow;
    }

    private static decimal ParseDecimal(string? raw, string decimalSeparator, string thousandsSeparator)
    {
        if (string.IsNullOrWhiteSpace(raw)) return 0;

        var sanitized = raw
            .Replace("$", string.Empty)
            .Replace("%", string.Empty)
            .Replace(thousandsSeparator, string.Empty)
            .Replace(decimalSeparator, ".")
            .Trim();

        return decimal.TryParse(sanitized,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var result)
            ? result
            : 0;
    }
}

public record StatementImportRow
{
    public DateTime Date { get; init; }
    public string Concept { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
