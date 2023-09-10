using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Helpers;
using FinanceApi.Infrastructure.Repositories;
using FinanceApi.Infrastructure.Repositories.Base;
using MediatR;

namespace FinanceApi.Application.Commands.Funds;

public class UploadFundFileCommandHandler : BaseResponselessHandler<UploadFundFileCommand>
{
    private readonly IAppModuleRepository appModuleRepository;
    private readonly IRepository<Movement, Guid> movementRepository;
    private readonly FundMovementExcelHelper excelHelper;

    public UploadFundFileCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
        this.movementRepository = movementRepository;
        this.excelHelper = new FundMovementExcelHelper();
    }

    public override async Task Handle(UploadFundFileCommand command, CancellationToken cancellationToken)
    {
        var appModule = await appModuleRepository.GetFund();

        var dateKind = command.DateKind;
        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;

        var newRecords = excelHelper.ReadAsync(command.File, appModule, dateKind);
        if (newRecords == null || !newRecords.Any()) return;

        var minDate = newRecords.Min(o => o.TimeStamp);
        var maxDate = newRecords.Max(o => o.TimeStamp);

        var timeStampProperty = "TimeStamp";
        var existingRecords = movementRepository
            .FilterBy(timeStampProperty, ExpressionOperator.GreaterThanOrEqual, minDate)
            .FilterBy(timeStampProperty, ExpressionOperator.LessThanOrEqual, maxDate)
            .Where(o => o.AppModule.Id == appModule.Id)
            .ToArray();

        newRecords = newRecords
            .Where(o => existingRecords.All(x =>
                x.AppModuleId != o.AppModuleId ||
                x.TimeStamp != o.TimeStamp ||
                x.Amount != o.Amount ||
                x.Total != o.Total ||
                x.Concept1 != o.Concept1 ||
                x.Concept2 != o.Concept2))
            .ToArray();

        await movementRepository.AddRange(newRecords, true);
    }
}

public class UploadFundFileCommand : IRequest
{
    public UploadFundFileCommand(IFormFile file, DateTimeKind dateKind)
    {
        this.File = file;
        this.DateKind = dateKind;
    }

    public IFormFile File { get; set; }
    public DateTimeKind DateKind { get; set; }
}
