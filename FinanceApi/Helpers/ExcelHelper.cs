using ExcelDataReader;
using FinanceApi.Domain.Models;

namespace FinanceApi.Helpers;

public class ExcelHelper
{
    public Movement[] ReadAsync(IEnumerable<IFormFile> files, AppModule module, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
    {
        return files.SelectMany(o => ReadAsync(o, module, dateTimeKind)).ToArray();
    }

    public Movement[] ReadAsync(IFormFile file, AppModule module, DateTimeKind dateTimeKind = DateTimeKind.Unspecified)
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
                        ModuleId = module.Id,
                        AppModule = module,
                        TimeStamp = ParsingHelper.ParseDateTime(sheet.Rows[r][0], "dd/MM/yyyy", null, dateTimeKind),
                        Concept1 = StringHelper.ValueOrEmpty(sheet.Rows[r][1]),
                        Concept2 = StringHelper.ValueOrEmpty(sheet.Rows[r][2]),
                        Amount = ParsingHelper.ParseDecimal(sheet.Rows[r][3]),
                        Total = ParsingHelper.ParseDecimal(sheet.Rows[r][4]),
                        Currency = module.Currency
                    });
                }
            }
        }

        return records.ToArray();
    }
}
