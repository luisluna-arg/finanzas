namespace Finance.Helpers.ExcelHelper;

public enum SourceType { Tabular, Pdf }

public class StatementImportConfig
{
    public SourceType SourceType { get; set; } = SourceType.Tabular;
    public int SkipRows { get; set; } = 1;
    public int SkipLastRows { get; set; } = 0;
    public int SkipPages { get; set; } = 0;
    public string DecimalSeparator { get; set; } = ",";
    public string ThousandsSeparator { get; set; } = ".";
    public bool AmountNegate { get; set; } = false;
    // UTC offset of the dates in the source file (e.g. -3 for Argentina).
    // Dates are stored as local-midnight expressed in UTC so the calendar date
    // is unambiguous even when DB/serialization converts to server local time.
    public double UtcOffsetHours { get; set; } = 0;
    public List<ColumnConfig> Columns { get; set; } = [];
}

public class ColumnConfig
{
    // Tabular (CSV/XLSX): 0-based column index
    public int Index { get; set; }
    // PDF: relative x bounds as fraction of page width (0–1)
    public double? XMin { get; set; }
    public double? XMax { get; set; }
    public string Name { get; set; } = string.Empty;
    public ColumnType Type { get; set; }
    public string? DateFormat { get; set; }
    public Guid? CurrencyId { get; set; }
    public string? InstallmentsPattern { get; set; }
}

public enum ColumnType
{
    Text,
    Date,
    Concept,
    Installment,
    Amount
}
