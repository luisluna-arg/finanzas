using FinanceApi.Application.Commands.Funds;
using FinanceApi.Domain;
using FinanceApi.Helpers;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Funds;

public class UploadFundFileCommandHandler : BaseResponselessHandler<UploadFundFileCommand>
{
    private readonly IAppModuleRepository appModuleRepository;

    public UploadFundFileCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
    }

    public override async Task Handle(UploadFundFileCommand command, CancellationToken cancellationToken)
    {
        var appModule = await appModuleRepository.GetFund();

        var excelHelper = new ExcelHelper();

        var dateKind = command.DateKind;
        var files = command.Files;

        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;
        var newRecords = excelHelper.ReadAsync(files, appModule, dateKind);

        if (newRecords == null || newRecords.Length == 0) return;

        var minDate = newRecords.Min(o => o.TimeStamp);
        var maxDate = newRecords.Max(o => o.TimeStamp);

        var existingRecords = DbContext.Movement.Where(o => o.TimeStamp >= minDate && o.TimeStamp <= maxDate && o.AppModule.Id == appModule.Id);

        newRecords = newRecords.Where(o => existingRecords.All(x =>
            x.AppModuleId != o.AppModuleId ||
                x.TimeStamp != o.TimeStamp ||
                x.Amount != o.Amount ||
                x.Total != o.Total ||
                x.Concept1 != o.Concept1 ||
                x.Concept2 != o.Concept2)).ToArray();

        DbContext.Movement.AddRange(newRecords);
        await DbContext.SaveChangesAsync();
    }
}
