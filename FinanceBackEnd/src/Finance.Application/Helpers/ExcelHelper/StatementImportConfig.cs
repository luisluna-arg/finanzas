namespace Finance.Helpers.ExcelHelper;

public class StatementImportConfig
{
    public int SkipRows { get; set; } = 1;
    public int SkipLastRows { get; set; } = 0;
    public int DateColumn { get; set; } = 0;
    public string DateFormat { get; set; } = "d/M/yyyy";
    public int ConceptColumn { get; set; } = 1;
    public int AmountColumn { get; set; } = 2;
    public Guid DefaultCurrencyId { get; set; }
    public string DecimalSeparator { get; set; } = ",";
    public string ThousandsSeparator { get; set; } = ".";
    public bool AmountNegate { get; set; } = false;
    public int? InstallmentsColumn { get; set; }
    public string? InstallmentsPattern { get; set; }
}
