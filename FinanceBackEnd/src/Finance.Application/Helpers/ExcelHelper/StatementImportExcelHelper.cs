using System.Text.RegularExpressions;
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

        var dateCol = config.Columns.FirstOrDefault(c => c.Type == ColumnType.Date);
        var conceptCol = config.Columns.FirstOrDefault(c => c.Type == ColumnType.Concept);
        var installmentCol = config.Columns.FirstOrDefault(c => c.Type == ColumnType.Installment);
        var amountCols = config.Columns.Where(c => c.Type == ColumnType.Amount).ToList();

        var rows = new List<StatementImportRow>();
        var lastRow = sheet.Rows.Count - config.SkipLastRows;

        for (int r = config.SkipRows; r < lastRow; r++)
        {
            var row = sheet.Rows[r];

            var dateRaw = dateCol != null ? row[dateCol.Index]?.ToString() : null;
            var conceptRaw = conceptCol != null ? row[conceptCol.Index]?.ToString() : null;

            if (string.IsNullOrWhiteSpace(dateRaw) && string.IsNullOrWhiteSpace(conceptRaw))
                continue;

            var date = ParseDate(dateRaw, dateCol?.DateFormat ?? "d/M/yyyy", config.UtcOffsetHours);
            var concept = conceptRaw ?? string.Empty;

            if (installmentCol != null)
            {
                var installments = row[installmentCol.Index]?.ToString()?.Trim();
                var isValid = !string.IsNullOrWhiteSpace(installments)
                    && (installmentCol.InstallmentsPattern is null
                        || Regex.IsMatch(installments, installmentCol.InstallmentsPattern));
                if (isValid)
                    concept = $"{concept} ({installments})";
            }

            foreach (var amountCol in amountCols)
            {
                var amountRaw = row[amountCol.Index]?.ToString();
                if (string.IsNullOrWhiteSpace(amountRaw)) continue;

                var amount = ParseDecimal(amountRaw, config.DecimalSeparator, config.ThousandsSeparator);
                if (amount == 0) continue;

                if (config.AmountNegate) amount = -amount;

                rows.Add(new StatementImportRow
                {
                    Date = date,
                    Concept = concept,
                    Amount = amount,
                    CurrencyId = amountCol.CurrencyId ?? Guid.Empty,
                });
            }
        }

        return rows;
    }

    private static DateTime ParseDate(string? raw, string format, double utcOffsetHours)
    {
        if (string.IsNullOrWhiteSpace(raw)) return DateTime.UtcNow;

        if (DateTime.TryParseExact(raw, format,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var parsed))
            return new DateTimeOffset(parsed, TimeSpan.FromHours(utcOffsetHours)).UtcDateTime;

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
    public Guid CurrencyId { get; init; }
}
