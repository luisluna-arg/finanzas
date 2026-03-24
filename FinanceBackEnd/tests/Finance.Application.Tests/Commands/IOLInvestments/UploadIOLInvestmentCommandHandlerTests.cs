using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Repositories;
using Finance.Domain.Enums;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Identities;
using Finance.Domain.Models.IOLInvestments;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Finance.Application.Tests.Queries.Base;

namespace Finance.Application.Tests.Commands.IOLInvestments;

public class UploadIOLInvestmentCommandHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IRepository<Currency, Guid>> _currencyRepository;
    private readonly Mock<IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum>> _iolInvestmentAssetTypeRepository;
    private readonly Mock<IExcelHelper<IOLInvestment>> _excelHelper;

    public UploadIOLInvestmentCommandHandlerTests()
    {
        _currencyRepository = new Mock<IRepository<Currency, Guid>>();
        _iolInvestmentAssetTypeRepository = new Mock<IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum>>();
        _excelHelper = new Mock<IExcelHelper<IOLInvestment>>();
    }

    private UploadIOLInvestmentCommandHandler CreateHandler() =>
        new(_dbContext, _currencyRepository.Object,
            new IOLInvestmentRepository(_dbContext),
            new IOLInvestmentAssetRepository(_dbContext),
            _iolInvestmentAssetTypeRepository.Object,
            _excelHelper.Object);

    [Fact]
    public async Task Upload_WhenHelperReturnsNoRecords_ReturnsFailure()
    {
        _excelHelper.Setup(h => h.Read(It.IsAny<IFormFile>(), It.IsAny<short?>()))
            .Returns(Array.Empty<IOLInvestment>());

        var result = await CreateHandler().ExecuteAsync(
            new UploadIOLInvestmentsCommand(StubFile()), default);

        Assert.False(result.IsSuccess);
        Assert.Equal("No records found in the uploaded file.", result.ErrorMessage);
    }

    [Fact]
    public async Task Upload_PassesTimezoneOffsetToHelper()
    {
        var record = MakeRecord();
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };
        _excelHelper.Setup(h => h.Read(It.IsAny<IFormFile>(), (short?)-3))
            .Returns([record]);

        _currencyRepository.Setup(r => r.GetByAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);
        _currencyRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _iolInvestmentAssetTypeRepository.Setup(r => r.GetByAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IOLInvestmentAssetType?)null);

        await CreateHandler().ExecuteAsync(
            new UploadIOLInvestmentsCommand(StubFile(), timezoneOffset: -3), default);

        _excelHelper.Verify(h => h.Read(It.IsAny<IFormFile>(), (short?)-3), Times.Once);
    }

    [Fact]
    public async Task Upload_WhenNoTimezoneProvided_PassesNullToHelper()
    {
        _excelHelper.Setup(h => h.Read(It.IsAny<IFormFile>(), (short?)null))
            .Returns(Array.Empty<IOLInvestment>());

        await CreateHandler().ExecuteAsync(
            new UploadIOLInvestmentsCommand(StubFile()), default);

        _excelHelper.Verify(h => h.Read(It.IsAny<IFormFile>(), (short?)null), Times.Once);
    }

    [Fact]
    public async Task Upload_WhenRecordAlreadyExists_SkipsIt()
    {
        var record = MakeRecord("AAPL");
        var user = await SeedUserAsync();

        var existingCurrency = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };
        var existingAsset = new IOLInvestmentAsset { Id = Guid.NewGuid(), Symbol = "AAPL", Description = string.Empty, TypeId = IOLInvestmentAssetTypeEnum.Cedear, CurrencyId = existingCurrency.Id };
        var existing = new IOLInvestment { Id = Guid.NewGuid(), Asset = existingAsset, AssetId = existingAsset.Id, TimeStamp = record.TimeStamp, CreatedAt = DateTime.UtcNow };
        _dbContext.Currency.Add(existingCurrency);
        _dbContext.IOLInvestmentAsset.Add(existingAsset);
        _dbContext.IOLInvestment.Add(existing);
        await _dbContext.SaveChangesAsync();
        await GrantInvestmentAccessAsync(user, existing);

        _excelHelper.Setup(h => h.Read(It.IsAny<IFormFile>(), It.IsAny<short?>()))
            .Returns([record]);

        var result = await CreateHandler().ExecuteAsync(
            new UploadIOLInvestmentsCommand(StubFile()), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, await _dbContext.IOLInvestment.IgnoreQueryFilters().CountAsync());
    }

    [Fact]
    public async Task Upload_WhenCurrencyNotFound_AndFallbackNotFound_SkipsRecord()
    {
        var record = MakeRecord("AAPL");

        _excelHelper.Setup(h => h.Read(It.IsAny<IFormFile>(), It.IsAny<short?>()))
            .Returns([record]);

        _currencyRepository.Setup(r => r.GetByAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Currency?)null);

        var result = await CreateHandler().ExecuteAsync(
            new UploadIOLInvestmentsCommand(StubFile()), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, await _dbContext.IOLInvestment.CountAsync());
    }

    [Fact]
    public async Task Upload_WhenNewRecord_AddsToRepository()
    {
        var record = MakeRecord("MSFT");
        var currency = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };

        _excelHelper.Setup(h => h.Read(It.IsAny<IFormFile>(), It.IsAny<short?>()))
            .Returns([record]);

        _currencyRepository.Setup(r => r.GetByAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);
        _currencyRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _iolInvestmentAssetTypeRepository.Setup(r => r.GetByAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IOLInvestmentAssetType?)null);

        var result = await CreateHandler().ExecuteAsync(
            new UploadIOLInvestmentsCommand(StubFile()), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, await _dbContext.IOLInvestment.IgnoreQueryFilters().CountAsync());
    }

    [Fact]
    public async Task Upload_WhenAssetAlreadyExists_ReusesIt()
    {
        var user = await SeedUserAsync();
        var existingCurrency = new Currency { Id = Guid.NewGuid(), Name = "Dollar", ShortName = "USD" };
        var existingAsset = new IOLInvestmentAsset { Id = Guid.NewGuid(), Symbol = "GOOGL", Description = string.Empty, TypeId = IOLInvestmentAssetTypeEnum.Cedear, CurrencyId = existingCurrency.Id };
        _dbContext.Currency.Add(existingCurrency);
        _dbContext.IOLInvestmentAsset.Add(existingAsset);
        await _dbContext.SaveChangesAsync();
        await GrantAssetAccessAsync(user, existingAsset);

        var record = MakeRecord("GOOGL");
        _excelHelper.Setup(h => h.Read(It.IsAny<IFormFile>(), It.IsAny<short?>()))
            .Returns([record]);

        _currencyRepository.Setup(r => r.GetByAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCurrency);
        _currencyRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCurrency);

        _iolInvestmentAssetTypeRepository.Setup(r => r.GetByAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IOLInvestmentAssetType?)null);

        await CreateHandler().ExecuteAsync(new UploadIOLInvestmentsCommand(StubFile()), default);

        var saved = await _dbContext.IOLInvestment.IgnoreQueryFilters().Include(i => i.Asset).FirstAsync();
        Assert.Equal(existingAsset.Id, saved.AssetId);
    }

    private static IFormFile StubFile() =>
        new FormFile(new MemoryStream([0x00]), 0, 1, "file", "investments.xlsx");

    private async Task<User> SeedUserAsync()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { Id = Guid.NewGuid(), SourceId = "IdentityNotFound" }],
        };
        _dbContext.User.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    private async Task GrantInvestmentAccessAsync(User user, IOLInvestment investment)
    {
        _dbContext.IOLInvestmentPermissions.Add(new IOLInvestmentPermissions
        {
            Id = Guid.NewGuid(),
            ResourceId = investment.Id,
            UserId = user.Id,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await _dbContext.SaveChangesAsync();
    }

    private async Task GrantAssetAccessAsync(User user, IOLInvestmentAsset asset)
    {
        _dbContext.IOLInvestmentAssetPermissions.Add(new IOLInvestmentAssetPermissions
        {
            Id = Guid.NewGuid(),
            ResourceId = asset.Id,
            UserId = user.Id,
            PermissionLevels = [PermissionLevelEnum.Owner],
        });
        await _dbContext.SaveChangesAsync();
    }

    private static IOLInvestment MakeRecord(string symbol = "AAPL", DateTime? timestamp = null)
    {
        var asset = new IOLInvestmentAsset
        {
            Id = Guid.NewGuid(),
            Symbol = symbol,
            Description = string.Empty,
            Type = new IOLInvestmentAssetType { Id = IOLInvestmentAssetTypeEnum.Cedear, Name = "Cedear" },
            Currency = Currency.Default(symbols: [CurrencySymbol.Default(symbol: "USD")]),
        };
        return new IOLInvestment
        {
            Id = Guid.NewGuid(),
            Asset = asset,
            TimeStamp = timestamp ?? new DateTime(2025, 6, 1, 12, 0, 0, DateTimeKind.Utc),
            CreatedAt = DateTime.UtcNow,
        };
    }
}

