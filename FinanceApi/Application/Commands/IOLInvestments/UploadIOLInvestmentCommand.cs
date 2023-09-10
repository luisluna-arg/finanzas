using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;
using MediatR;
using static FinanceApi.Core.Config.DatabaseSeeder;

namespace FinanceApi.Application.Commands.IOLInvestments;

public class UploadIOLInvestmentCommandHandler : BaseResponselessHandler<UploadIOLInvestmentsCommand>
{
    private readonly IRepository<IOLInvestment, Guid> repository;
    private readonly IRepository<IOLInvestmentAsset, Guid> assetRepository;
    private readonly IAppModuleRepository appModuleRepository;
    private readonly IOLInvestmentExcelHelper excelHelper;

    public UploadIOLInvestmentCommandHandler(
        FinanceDbContext db,
        IRepository<IOLInvestment, Guid> investmentAssetIOLRecordRepository,
        IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLRepository,
        IAppModuleRepository appModuleRepository)
        : base(db)
    {
        repository = investmentAssetIOLRecordRepository;
        assetRepository = investmentAssetIOLRepository;
        this.appModuleRepository = appModuleRepository;
        excelHelper = new IOLInvestmentExcelHelper();
    }

    public override async Task Handle(UploadIOLInvestmentsCommand command, CancellationToken cancellationToken)
    {
        var files = command.File;

        var appModule = await appModuleRepository.GetBy("Name", AppModuleNames.IOLInvestments);

        if (appModule == null) throw new Exception($"App module {AppModuleNames.IOLInvestments} not found");

        var newRecords = excelHelper.ReadAsync(files, appModule, DateTimeKind.Utc).ToArray();

        if (newRecords.Length > 0)
        {
            var assets = new Dictionary<string, IOLInvestmentAsset>();

            foreach (var record in newRecords)
            {
                var asset = assets.ContainsKey(record.Asset.Symbol) ?
                    assets[record.Asset.Symbol] :
                    await assetRepository.GetBy("Symbol", record.Asset.Symbol);

                if (asset != null)
                {
                    record.Asset = asset;
                    assets.Add(asset.Symbol, asset);
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
