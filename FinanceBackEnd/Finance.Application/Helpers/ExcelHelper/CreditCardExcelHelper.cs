using ExcelDataReader;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Finance.Helpers.ExcelHelper;

public class CreditCardExcelHelper : ICreditCardExcelHelper<CreditCardMovement>
{
    public IEnumerable<CreditCardMovement> Read(IEnumerable<IFormFile> files, CreditCard creditCard, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        return files.SelectMany(o => Read(o, creditCard, dateTimeKind)).ToArray();
    }

    public IEnumerable<CreditCardMovement> Read(IFormFile file, CreditCard creditCard, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        List<CreditCardMovement> records = new List<CreditCardMovement>();

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
                var dateString = sheet.TableName.Split("_").Last().Split("-");

                var currentDate = new DateTime(int.Parse(dateString[2]), int.Parse(dateString[1]), int.Parse(dateString[0]), 0, 0, 0, DateTimeKind.Local).ToUniversalTime();

                for (var r = 3; r < sheet.Rows.Count; r++)
                {
                    if (!string.IsNullOrWhiteSpace(sheet.Rows[r][0].ToString())) break;

                    records.Add(new CreditCardMovement()
                    {
                        CreditCard = creditCard,
                        TimeStamp = currentDate,
                        PlanStart = DateTimeParser(sheet.Rows[r][1], dateTimeKind),
                        Concept = sheet.Rows[r][2]?.ToString() ?? "-",
                        PaymentNumber = GetPaymentNumber(sheet.Rows[r][3]),
                        PlanSize = GetPlanSize(sheet.Rows[r][3]),
                        Amount = DecimalParser(sheet.Rows[r][4]),
                        AmountDollars = DecimalParser(sheet.Rows[r][5]),
                    });
                }
            }
        }

        return records.ToArray();
    }

    private ushort GetPaymentNumber(object cell)
    {
        var stringValue = cell?.ToString()?.Split("/");
        if (stringValue == null || stringValue.Length == 0 || stringValue.All(o => o.Trim().Equals(string.Empty))) return 1;

        return UShortParser(stringValue[1]);
    }

    private ushort GetPlanSize(object cell)
    {
        var stringValue = cell?.ToString()?.Split("/");
        if (stringValue == null || stringValue.Length == 0 || stringValue.All(o => o.Trim().Equals(string.Empty))) return 1;

        return UShortParser(stringValue[0]);
    }

    private DateTime DateTimeParser(object cell, DateTimeKind dateTimeKind)
    {
        var dateString = cell?.ToString();
        if (string.IsNullOrWhiteSpace(dateString)) return default;

        var parsedDateTime = DateTimeHelper.ParseDateTime(dateString, "d/M/yyyy", null, dateTimeKind);
        if (parsedDateTime.Ticks == DateTime.MinValue.Ticks) parsedDateTime = DateTime.Now;
        return DateTimeHelper.FromTimeZoneToUTC(parsedDateTime, -3);
    }

    private decimal DecimalParser(object cell)
        => ParsingHelper.ParseDecimal(SanitizeDecimalString(cell));

    private ushort UShortParser(string cell)
        => ParsingHelper.ParseUShort(cell);

    private string? SanitizeDecimalString(object value)
        => value?.ToString()?
            .Replace("$", string.Empty)
            .Replace("%", string.Empty)
            .Replace(".", string.Empty)
            .Replace(",", ".")
            .Trim();
}
