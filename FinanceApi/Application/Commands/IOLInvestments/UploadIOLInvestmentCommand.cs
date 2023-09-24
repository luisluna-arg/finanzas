using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Commands.IOLInvestments;

public class UploadIOLInvestmentCommandHandler : BaseResponselessHandler<UploadIOLInvestmentsCommand>
{
    private readonly IRepository<IOLInvestment, Guid> repository;
    private readonly IRepository<IOLInvestmentAsset, Guid> assetRepository;
    private readonly IRepository<IOLInvestmentAssetType, ushort> assetTypeRepository;
    private readonly IOLInvestmentExcelHelper excelHelper;

    public UploadIOLInvestmentCommandHandler(
        FinanceDbContext db,
        IRepository<IOLInvestment, Guid> investmentAssetIOLRecordRepository,
        IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLRepository,
        IRepository<IOLInvestmentAssetType, ushort> investmentAssetTypeIOLRepository)
        : base(db)
    {
        repository = investmentAssetIOLRecordRepository;
        assetRepository = investmentAssetIOLRepository;
        assetTypeRepository = investmentAssetTypeIOLRepository;
        excelHelper = new IOLInvestmentExcelHelper();
    }

    public override async Task Handle(UploadIOLInvestmentsCommand command, CancellationToken cancellationToken)
    {
        var files = command.File;

        var newRecords = excelHelper.Read(files, DateTimeKind.Utc).ToArray();

        if (newRecords.Length > 0)
        {
            var singleRecord = newRecords.First();

            var records = await repository.GetAllBy("TimeStamp", singleRecord.TimeStamp)
                .Include(o => o.Asset)
                .ToArrayAsync();

            var assets = new Dictionary<string, IOLInvestmentAsset>();
            var assetTypes = new Dictionary<string, IOLInvestmentAssetType>();

            foreach (var record in newRecords)
            {
                var existingRecord = records.FirstOrDefault(x => x.Asset.Symbol == record.Asset.Symbol);
                if (existingRecord != null) continue;

                var assetType = assetTypes.ContainsKey(record.Asset.Type.Name) ?
                    assetTypes[record.Asset.Type.Name] :
                    await assetTypeRepository.GetBy("Name", record.Asset.Type.Name);

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
                    await assetRepository.GetBy("Symbol", record.Asset.Symbol);

                if (asset != null)
                {
                    record.Asset = asset;
                }
                else
                {
                    assets.Add(record.Asset.Symbol, record.Asset);
                }

                await repository.Add(record, false);
            }

            await repository.Persist();
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
