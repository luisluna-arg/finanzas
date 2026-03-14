using System.Runtime.Versioning;
using System.Text;
using Finance.Application.Commands.Funds;
using Finance.Application.Repositories;
using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Movements;
using Finance.Helpers;
using Finance.Persistence;
using Finance.Application.Repositories.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Finance.Application.Tests.Commands.Funds;

[SupportedOSPlatform("windows")]
public class FundFileAndImageCommandHandlerTests : IDisposable
{
    private readonly FinanceDbContext _dbContext;

    public FundFileAndImageCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new FinanceDbContext(options, null);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task UploadImage_WhenFundModuleExists_ReturnsSuccess()
    {
        var appModuleRepository = new Mock<IAppModuleRepository>();
        appModuleRepository.Setup(r => r.GetFundsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new AppModule());

        var handler = new UploadImageCommandHandler(_dbContext, appModuleRepository.Object);

        var result = await handler.ExecuteAsync(new UploadImageCommand { Files = new FormFileCollection() }, default);

        Assert.True(result.IsSuccess);
        appModuleRepository.Verify(r => r.GetFundsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadFundFile_WhenBankDoesNotExist_ThrowsException()
    {
        var appModuleRepository = new Mock<IAppModuleRepository>();
        var movementRepository = new Mock<IRepository<Movement, Guid>>();
        var bankRepository = new Mock<IRepository<Bank, Guid>>();
        var excelHelper = new Mock<IFundsExcelHelper<Movement>>();
        var command = new UploadFundFileCommand(CreateFormFile("test.xlsx"), Guid.NewGuid(), DateTimeKind.Utc);

        appModuleRepository.Setup(r => r.GetFundsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new AppModule());
        bankRepository.Setup(r => r.GetByAsync("Id", command.BankId, It.IsAny<CancellationToken>())).ReturnsAsync((Bank?)null);

        var handler = new UploadFundFileCommandHandler(_dbContext, appModuleRepository.Object, movementRepository.Object, bankRepository.Object, excelHelper.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.ExecuteAsync(command, default));
    }

    [Fact]
    public async Task UploadFundFile_WhenNoRecordsAreRead_ReturnsFailure()
    {
        var appModule = new AppModule { Id = Guid.NewGuid(), Currency = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" }, Name = "Funds" };
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Galicia" };
        var appModuleRepository = new Mock<IAppModuleRepository>();
        var movementRepository = new Mock<IRepository<Movement, Guid>>();
        var bankRepository = new Mock<IRepository<Bank, Guid>>();
        var excelHelper = new Mock<IFundsExcelHelper<Movement>>();
        var command = new UploadFundFileCommand(CreateFormFile("test.xlsx"), bank.Id, DateTimeKind.Utc);

        appModuleRepository.Setup(r => r.GetFundsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(appModule);
        bankRepository.Setup(r => r.GetByAsync("Id", bank.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bank);
        excelHelper.Setup(h => h.Read(It.IsAny<IFormFile>(), appModule, bank, DateTimeKind.Utc)).Returns(Array.Empty<Movement>());

        var handler = new UploadFundFileCommandHandler(_dbContext, appModuleRepository.Object, movementRepository.Object, bankRepository.Object, excelHelper.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.False(result.IsSuccess);
        Assert.Equal("No records found in the uploaded file.", result.ErrorMessage);
    }

    [Fact]
    public async Task UploadFundFile_WhenRecordsAreRead_AddsThemToRepository()
    {
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Peso", ShortName = "ARS" };
        var appModule = new AppModule { Id = Guid.NewGuid(), Currency = currency, Name = "Funds" };
        var bank = new Bank { Id = Guid.NewGuid(), Name = "Galicia" };
        var appModuleRepository = new Mock<IAppModuleRepository>();
        var movementRepository = new Mock<IRepository<Movement, Guid>>();
        var bankRepository = new Mock<IRepository<Bank, Guid>>();
        var excelHelper = new Mock<IFundsExcelHelper<Movement>>();
        var command = new UploadFundFileCommand(CreateFormFile("test.xlsx"), bank.Id, DateTimeKind.Utc);
        var newRecords = new[]
        {
            new Movement { Id = Guid.NewGuid(), AppModule = appModule, AppModuleId = appModule.Id, Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc), Concept1 = "A", Amount = 100m },
            new Movement { Id = Guid.NewGuid(), AppModule = appModule, AppModuleId = appModule.Id, Bank = bank, BankId = bank.Id, Currency = currency, CurrencyId = currency.Id, TimeStamp = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc), Concept1 = "B", Amount = 200m },
        };

        appModuleRepository.Setup(r => r.GetFundsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(appModule);
        bankRepository.Setup(r => r.GetByAsync("Id", bank.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bank);
        excelHelper.Setup(h => h.Read(It.IsAny<IFormFile>(), appModule, bank, DateTimeKind.Utc)).Returns(newRecords);
        movementRepository.Setup(r => r.FilterBy(It.IsAny<string>(), It.IsAny<ExpressionOperator>(), It.IsAny<object>())).Returns(Array.Empty<Movement>().AsQueryable());

        var handler = new UploadFundFileCommandHandler(_dbContext, appModuleRepository.Object, movementRepository.Object, bankRepository.Object, excelHelper.Object);

        var result = await handler.ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        movementRepository.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Movement>>(m => m.Count() == 2), It.IsAny<CancellationToken>(), true), Times.Once);
    }

    [Fact]
    public async Task ProcessImage_WritesAdjustedBytesToResponse()
    {
        var ocrHelper = new Mock<IOcrHelper>();
        var responseStream = new MemoryStream();
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = responseStream;
        var expectedBytes = Encoding.UTF8.GetBytes("image-bytes");
        ocrHelper.Setup(h => h.AdjustImage(It.IsAny<MemoryStream>())).Returns(new MemoryStream(expectedBytes));

        var handler = new ProcessImageCommandHandler(_dbContext, ocrHelper.Object);
        var result = await handler.ExecuteAsync(new ProcessImageCommand
        {
            HttpContext = httpContext,
            Files = new FormFileCollection { CreateFormFile("image.png", "source") },
        }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal("image/jpeg", httpContext.Response.ContentType);
        Assert.Equal("attachment; filename=image.jpg", httpContext.Response.Headers.ContentDisposition.ToString());

        responseStream.Position = 0;
        Assert.Equal("image-bytes", new StreamReader(responseStream).ReadToEnd());
    }

    [Fact]
    public async Task ProcessImageToText_WritesGeneratedTextToResponse()
    {
        var ocrHelper = new Mock<IOcrHelper>();
        var responseStream = new MemoryStream();
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = responseStream;
        ocrHelper.Setup(h => h.CitricCaptureToImage(It.IsAny<IEnumerable<IFormFile>>())).Returns(["line 1", "line 2"]);

        var handler = new ProcessImageToTextCommandHandler(_dbContext, ocrHelper.Object);
        var result = await handler.ExecuteAsync(new ProcessImageToTextCommand
        {
            HttpContext = httpContext,
            Files = new FormFileCollection { CreateFormFile("image.png", "source") },
        }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal("text/plain", httpContext.Response.ContentType);
        Assert.StartsWith("attachment; filename=\"Lemon_", httpContext.Response.Headers.ContentDisposition.ToString(), StringComparison.Ordinal);

        responseStream.Position = 0;
        var text = new StreamReader(responseStream).ReadToEnd();
        Assert.Contains("line 1", text);
        Assert.Contains("line 2", text);
    }

    private static IFormFile CreateFormFile(string fileName, string content = "content")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName);
    }
}
