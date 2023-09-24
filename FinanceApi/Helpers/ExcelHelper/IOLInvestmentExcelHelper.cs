using ExcelDataReader;
using FinanceApi.Domain.Models;
using FinanceApi.Helpers;

public class IOLInvestmentExcelHelper : IExcelHelper<IOLInvestment>
{
    public IEnumerable<IOLInvestment> Read(IEnumerable<IFormFile> files, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
        => files.SelectMany(o => Read(o, dateTimeKind)).ToArray();

    public IEnumerable<IOLInvestment> Read(IFormFile file, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        List<IOLInvestment> records = new List<IOLInvestment>();

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

                Func<object, uint> uIntParser = (cell) => ParsingHelper.ParseUInteger(SanitizeDecimalString(cell));
                Func<object, decimal> decimalParser = (cell) => ParsingHelper.ParseDecimal(SanitizeDecimalString(cell));

                var dateString = sheet.Rows[0][1].ToString();
                var currentDate = DateTimeHelper.ParseDateTime(dateString, "d/M/yyyy HH:mm:ss", null, dateTimeKind);
                if (currentDate.Ticks == DateTime.MinValue.Ticks) currentDate = DateTime.Now;
                currentDate = DateTimeHelper.FromTimeZoneToUTC(currentDate, -3);

                for (var r = 2; r < sheet.Rows.Count; r++)
                {
                    var row = sheet.Rows[r];
                    var asset = StringHelper.ValueOrEmpty(row[0]).Split("\n");
                    var assetSymbol = asset[0].Trim();
                    var assetDescription = asset.Length == 2 ? asset[1] : string.Empty;

                    records.Add(new IOLInvestment()
                    {
                        Asset = new IOLInvestmentAsset()
                        {
                            Symbol = assetSymbol,
                            Description = assetDescription,
                            Type = new IOLInvestmentAssetType()
                            {
                                Name = IOLInvestmentAssetType.Default
                            }
                        },
                        TimeStamp = currentDate,
                        Alarms = uIntParser(row[1]),
                        Quantity = uIntParser(row[2]),
                        Assets = uIntParser(row[3]),
                        DailyVariation = decimalParser(row[4]),
                        LastPrice = decimalParser(row[5]),
                        AverageBuyPrice = decimalParser(row[6]),
                        AverageReturnPercent = decimalParser(row[7]),
                        AverageReturn = decimalParser(row[8]),
                        Valued = decimalParser(row[9])
                    });
                }
            }
        }

        return records.ToArray();
    }

    private object? SanitizeDecimalString(object value)
        => value?.ToString()?
            .Replace("$", string.Empty)
            .Replace("%", string.Empty)
            .Replace(".", string.Empty)
            .Replace(",", ".")
            .Trim();
}
