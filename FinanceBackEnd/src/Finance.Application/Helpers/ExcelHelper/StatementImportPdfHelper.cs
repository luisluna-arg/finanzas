using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Finance.Helpers.ExcelHelper;

public class StatementImportPdfHelper
{
    private const double LineYTolerance = 4.0;

    public IEnumerable<StatementImportRow> Read(IFormFile file, StatementImportConfig config)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File not selected");

        var dateCol = config.Columns.FirstOrDefault(c => c.Type == ColumnType.Date);
        var conceptCol = config.Columns.FirstOrDefault(c => c.Type == ColumnType.Concept);
        var amountCols = config.Columns.Where(c => c.Type == ColumnType.Amount).ToList();

        var rows = new List<StatementImportRow>();

        using var stream = file.OpenReadStream();
        using var document = PdfDocument.Open(stream);

        for (int pageNum = config.SkipPages + 1; pageNum <= document.NumberOfPages; pageNum++)
        {
            var page = document.GetPage(pageNum);
            var pageWidth = page.Width;
            var lines = GroupIntoLines(page.GetWords());

            foreach (var line in lines)
            {
                if (dateCol == null) continue;
                var dateText = ExtractColumnText(line, dateCol, pageWidth);
                if (!TryParseDate(dateText, dateCol.DateFormat ?? "dd/MM/yy", config.UtcOffsetHours, out var date))
                    continue;

                var concept = conceptCol != null
                    ? ExtractColumnText(line, conceptCol, pageWidth)
                    : string.Empty;

                if (string.IsNullOrWhiteSpace(concept))
                    continue;

                foreach (var amountCol in amountCols)
                {
                    var amountText = ExtractColumnText(line, amountCol, pageWidth);
                    if (string.IsNullOrWhiteSpace(amountText)) continue;

                    var amount = ParseDecimal(amountText, config.DecimalSeparator, config.ThousandsSeparator);
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
        }

        return rows;
    }

    public PdfDiagnosticResult Diagnose(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File not selected");

        using var stream = file.OpenReadStream();
        using var document = PdfDocument.Open(stream);

        var pages = new List<PdfDiagnosticPage>();
        for (int pageNum = 1; pageNum <= document.NumberOfPages; pageNum++)
        {
            var page = document.GetPage(pageNum);
            var w = page.Width;
            var h = page.Height;
            var lines = GroupIntoLines(page.GetWords());

            var diagLines = lines.Select(line => new PdfDiagnosticLine
            {
                Text = string.Join(" ", line.Select(word => word.Text)),
                Words = line.Select(word => new PdfDiagnosticWord
                {
                    Text = word.Text,
                    XCenter = Math.Round(word.BoundingBox.Centroid.X / w, 4),
                    YCenter = Math.Round(word.BoundingBox.Centroid.Y / h, 4),
                    XMin = Math.Round(word.BoundingBox.Left / w, 4),
                    XMax = Math.Round(word.BoundingBox.Right / w, 4),
                }).ToList(),
            }).ToList();

            pages.Add(new PdfDiagnosticPage { PageNumber = pageNum, Width = w, Height = h, Lines = diagLines });
        }

        return new PdfDiagnosticResult { PageCount = document.NumberOfPages, Pages = pages };
    }

    // Groups words into lines by proximity of their y-centers (PDF y=0 is bottom).
    // Returns lines sorted top-to-bottom (descending y).
    private static List<List<Word>> GroupIntoLines(IEnumerable<Word> words)
    {
        var sorted = words
            .OrderByDescending(w => w.BoundingBox.Centroid.Y)
            .ToList();

        var lines = new List<List<Word>>();
        foreach (var word in sorted)
        {
            var wordY = word.BoundingBox.Centroid.Y;
            var matched = lines.FirstOrDefault(line =>
                Math.Abs(wordY - line[0].BoundingBox.Centroid.Y) <= LineYTolerance);

            if (matched != null)
                matched.Add(word);
            else
                lines.Add([word]);
        }

        foreach (var line in lines)
            line.Sort((a, b) => a.BoundingBox.Left.CompareTo(b.BoundingBox.Left));

        return lines;
    }

    private static string ExtractColumnText(List<Word> line, ColumnConfig col, double pageWidth)
    {
        if (col.XMin == null || col.XMax == null) return string.Empty;
        var xMin = col.XMin.Value * pageWidth;
        var xMax = col.XMax.Value * pageWidth;

        var words = line
            .Where(w => w.BoundingBox.Centroid.X >= xMin && w.BoundingBox.Centroid.X <= xMax)
            .OrderBy(w => w.BoundingBox.Left);

        return string.Join(" ", words.Select(w => w.Text)).Trim();
    }

    private static bool TryParseDate(string? raw, string format, double utcOffsetHours, out DateTime date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(raw)) return false;

        if (DateTime.TryParseExact(raw, format,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var parsed))
        {
            date = new DateTimeOffset(parsed, TimeSpan.FromHours(utcOffsetHours)).UtcDateTime;
            return true;
        }
        return false;
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

public record PdfDiagnosticResult
{
    public int PageCount { get; init; }
    public List<PdfDiagnosticPage> Pages { get; init; } = [];
}

public record PdfDiagnosticPage
{
    public int PageNumber { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
    public List<PdfDiagnosticLine> Lines { get; init; } = [];
}

public record PdfDiagnosticLine
{
    public string Text { get; init; } = string.Empty;
    public List<PdfDiagnosticWord> Words { get; init; } = [];
}

public record PdfDiagnosticWord
{
    public string Text { get; init; } = string.Empty;
    public double XCenter { get; init; }
    public double YCenter { get; init; }
    public double XMin { get; init; }
    public double XMax { get; init; }
}
