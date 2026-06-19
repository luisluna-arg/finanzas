namespace Finance.Helpers.ExcelHelper;

public class StatementImportConfig
{
    public int SkipRows { get; set; } = 1;
    public int SkipLastRows { get; set; } = 0;
    public string DecimalSeparator { get; set; } = ",";
    public string ThousandsSeparator { get; set; } = ".";
    public bool AmountNegate { get; set; } = false;
    public List<ColumnConfig> Columns { get; set; } = [];
}

public class ColumnConfig
{
    public int Index { get; set; }
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
