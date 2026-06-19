using Finance.Helpers.ExcelHelper;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Finance.Application.Tests.Helpers;

public class StatementImportExcelHelperTests
{
    static StatementImportExcelHelperTests()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    private static readonly Guid TestCurrencyId = Guid.NewGuid();

    private static StatementImportConfig DefaultConfig() => new()
    {
        SkipRows = 1,
        DecimalSeparator = ",",
        ThousandsSeparator = ".",
        AmountNegate = false,
        Columns =
        [
            new() { Index = 0, Name = "Fecha", Type = ColumnType.Date, DateFormat = "d/M/yyyy" },
            new() { Index = 1, Name = "Concepto", Type = ColumnType.Concept },
            new() { Index = 2, Name = "Monto", Type = ColumnType.Amount, CurrencyId = TestCurrencyId },
        ],
    };

    private static IFormFile BuildCsvFile(string content, string fileName = "test.csv")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        var file = new Mock<IFormFile>();
        file.Setup(f => f.FileName).Returns(fileName);
        file.Setup(f => f.Length).Returns(stream.Length);
        file.Setup(f => f.OpenReadStream()).Returns(stream);
        return file.Object;
    }

    [Fact]
    public void Read_ValidCsv_ParsesRowsCorrectly()
    {
        var csv = "Date,Concept,Amount\r\n1/6/2025,Netflix,1500\r\n5/6/2025,Uber,300";
        var file = BuildCsvFile(csv);
        var helper = new StatementImportExcelHelper();

        var rows = helper.Read(file, DefaultConfig()).ToArray();

        Assert.Equal(2, rows.Length);
        Assert.Equal(new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), rows[0].Date);
        Assert.Equal("Netflix", rows[0].Concept);
        Assert.Equal(1500m, rows[0].Amount);
        Assert.Equal(new DateTime(2025, 6, 5, 0, 0, 0, DateTimeKind.Utc), rows[1].Date);
        Assert.Equal("Uber", rows[1].Concept);
        Assert.Equal(300m, rows[1].Amount);
    }

    [Fact]
    public void Read_NullFile_ThrowsException()
    {
        var helper = new StatementImportExcelHelper();

        Assert.Throws<Exception>(() => helper.Read(null!, DefaultConfig()).ToArray());
    }

    [Fact]
    public void Read_EmptyFile_ThrowsException()
    {
        var file = new Mock<IFormFile>();
        file.Setup(f => f.FileName).Returns("test.csv");
        file.Setup(f => f.Length).Returns(0);
        var helper = new StatementImportExcelHelper();

        Assert.Throws<Exception>(() => helper.Read(file.Object, DefaultConfig()).ToArray());
    }

    [Fact]
    public void Read_UnsupportedExtension_ThrowsException()
    {
        var file = BuildCsvFile("a,b,c", "test.txt");
        var helper = new StatementImportExcelHelper();

        Assert.Throws<Exception>(() => helper.Read(file, DefaultConfig()).ToArray());
    }

    [Fact]
    public void Read_SkipRows_OmitsHeaderRow()
    {
        var csv = "Date,Concept,Amount\r\n1/6/2025,Netflix,1500";
        var file = BuildCsvFile(csv);
        var helper = new StatementImportExcelHelper();

        var rows = helper.Read(file, DefaultConfig()).ToArray();

        Assert.Single(rows);
        Assert.Equal("Netflix", rows[0].Concept);
    }

    [Fact]
    public void Read_AmountNegate_NegatesValues()
    {
        var csv = "Date,Concept,Amount\r\n1/6/2025,Netflix,1500";
        var file = BuildCsvFile(csv);
        var config = DefaultConfig();
        config.AmountNegate = true;
        var helper = new StatementImportExcelHelper();

        var rows = helper.Read(file, config).ToArray();

        Assert.Equal(-1500m, rows[0].Amount);
    }

    [Fact]
    public void Read_RowWithEmptyDateAndConcept_IsSkipped()
    {
        var csv = "Date,Concept,Amount\r\n,,100\r\n1/6/2025,Netflix,1500";
        var file = BuildCsvFile(csv);
        var helper = new StatementImportExcelHelper();

        var rows = helper.Read(file, DefaultConfig()).ToArray();

        Assert.Single(rows);
        Assert.Equal("Netflix", rows[0].Concept);
    }

    [Fact]
    public void Read_DollarSignInAmount_StripsCurrencySymbol()
    {
        var csv = "Date,Concept,Amount\r\n1/6/2025,Netflix,$1500";
        var config = DefaultConfig();
        config.DecimalSeparator = ".";
        config.ThousandsSeparator = ",";
        var file = BuildCsvFile(csv);
        var helper = new StatementImportExcelHelper();

        var rows = helper.Read(file, config).ToArray();

        Assert.Equal(1500m, rows[0].Amount);
    }

    [Fact]
    public void Read_CommaDecimalSeparator_ParsesDecimalCorrectly()
    {
        var csv = "Date,Concept,Amount\r\n1/6/2025,Netflix,\"1.500,75\"";
        var config = DefaultConfig();
        config.DecimalSeparator = ",";
        config.ThousandsSeparator = ".";
        var file = BuildCsvFile(csv);
        var helper = new StatementImportExcelHelper();

        var rows = helper.Read(file, config).ToArray();

        Assert.Equal(1500.75m, rows[0].Amount);
    }
}
