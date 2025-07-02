using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Domain.Enums;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Finance.Persistance.Constants;

namespace Finance.Application.Commands.IOLInvestments;

public class UploadIOLInvestmentCommandHandler : BaseResponselessHandler<UploadIOLInvestmentsCommand>
{
    private readonly IRepository<Currency, Guid> _currencyRepository;
    private readonly IRepository<IOLInvestment, Guid> _iolInvestment;
    private readonly IRepository<IOLInvestmentAsset, Guid> _iolInvestmentAsset;
    private readonly IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> _iolInvestmentAssetType;
    private readonly IOLInvestmentExcelHelper _iolInvestmentExcelHelper;

    public UploadIOLInvestmentCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<IOLInvestment, Guid> iolInvestmentRepository,
        IRepository<IOLInvestmentAsset, Guid> iolInvestmentAssetRepository,
        IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> iolInvestmentAssetTypeRepository,
        IOLInvestmentExcelHelper iolInvestmentExcelHelper)
        : base(db)
    {
        _iolInvestment = iolInvestmentRepository;
        _currencyRepository = currencyRepository;
        _iolInvestmentAsset = iolInvestmentAssetRepository;
        _iolInvestmentAssetType = iolInvestmentAssetTypeRepository;
        _iolInvestmentExcelHelper = iolInvestmentExcelHelper;
    }

    public override async Task Handle(UploadIOLInvestmentsCommand command, CancellationToken cancellationToken)
    {
        var files = command.File;

        var newRecords = _iolInvestmentExcelHelper.Read(files, DateTimeKind.Utc).ToArray();

        if (newRecords.Length > 0)
        {
            var singleRecord = newRecords.First();

            var records = await _iolInvestment.GetAllBy("TimeStamp", singleRecord.TimeStamp)
                .Include(o => o.Asset)
                .ToArrayAsync();

            var assets = new Dictionary<string, IOLInvestmentAsset>();
            var assetTypes = new Dictionary<string, IOLInvestmentAssetType>();

            foreach (var record in newRecords)
            {
                var existingRecord = records.FirstOrDefault(x => x.Asset.Symbol == record.Asset.Symbol);
                if (existingRecord != null) continue;

                var assetCurrency = await _currencyRepository.GetByAsync("Symbol", record.Asset.Currency!.Symbols.Select(s => s.Symbol), cancellationToken) ??
                    await _currencyRepository.GetByIdAsync(Guid.Parse(CurrencyConstants.PesoId), cancellationToken);
                if (assetCurrency == null) continue;

                var assetType = assetTypes.ContainsKey(record.Asset.Type.Name) ?
                    assetTypes[record.Asset.Type.Name] :
                    await _iolInvestmentAssetType.GetByAsync("Name", record.Asset.Type.Name, cancellationToken);

                if (assetType != null)
                {
                    record.Asset.Type = assetType;
                }
                else
                {
                    assetTypes.Add(record.Asset.Type.Name, record.Asset.Type);
                }

                var asset = assets.ContainsKey(record.Asset.Symbol) ?
                    assets[record.Asset.Symbol] :
                    await _iolInvestmentAsset.GetByAsync("Symbol", record.Asset.Symbol, cancellationToken);

                if (asset != null)
                {
                    record.Asset = asset;
                }
                else
                {
                    record.Asset.Currency = assetCurrency;

                    assets.Add(record.Asset.Symbol, record.Asset);
                }

                await _iolInvestment.AddAsync(record, cancellationToken, false);
            }

            await _iolInvestment.PersistAsync(cancellationToken);
        }
    }
}

public class UploadIOLInvestmentsCommand : IRequest
{
    public UploadIOLInvestmentsCommand(IFormFile file)
    {
        this.File = file;
    }

    public IFormFile File { get; set; }
}
