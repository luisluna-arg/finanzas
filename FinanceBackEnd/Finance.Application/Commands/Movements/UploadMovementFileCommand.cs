using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Domain.Comparers;
using Finance.Domain.Models;
using Finance.Helpers;
using Finance.Application.Repositories;
using Finance.Application.Repositories.Base;
using Finance.Persistance;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Commands.Movements;

public class UploadMovementsFileCommandHandler : BaseResponselessHandler<UploadMovementsFileCommand>
{
    private readonly IAppModuleRepository appModuleRepository;
    private readonly IRepository<Movement, Guid> movementRepository;
    private readonly IRepository<Bank, Guid> bankRepository;
    private readonly FundsExcelHelper excelHelper;

    public UploadMovementsFileCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<Movement, Guid> movementRepository,
        IRepository<Bank, Guid> bankRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
        this.movementRepository = movementRepository;
        this.bankRepository = bankRepository;
        this.excelHelper = new FundsExcelHelper();
    }

    public override async Task<CommandResult> ExecuteAsync(UploadMovementsFileCommand request, CancellationToken cancellationToken)
    {
        var appModule = await appModuleRepository.GetByAsync("Id", request.AppModuleId, cancellationToken);
        if (appModule == null) throw new Exception($"App Module not found, Id: {request.AppModuleId}");

        var bank = await bankRepository.GetByAsync("Id", request.BankId, cancellationToken);
        if (bank == null) throw new Exception($"Bank not found, Id: {request.BankId}");

        var dateKind = request.DateKind;
        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;

        var newRecords = excelHelper.Read(request.File, appModule, bank, dateKind);
        if (newRecords == null || !newRecords.Any()) return CommandResult.Failure("No records found in the uploaded file.");

        var minDate = newRecords.Min(o => o.TimeStamp);
        var maxDate = newRecords.Max(o => o.TimeStamp);

        var timeStampProperty = "TimeStamp";
        var existingRecords = movementRepository
            .FilterBy(timeStampProperty, ExpressionOperator.GreaterThanOrEqual, minDate)
            .FilterBy(timeStampProperty, ExpressionOperator.LessThanOrEqual, maxDate)
            .Where(o => o.AppModule.Id == appModule.Id)
            .Where(o => o.Bank != null && o.Bank!.Id == bank.Id)
            .ToArray();

        var movementComparer = new MovementComparer();
        newRecords = newRecords
            .Where(o => existingRecords.All(x => !movementComparer.Equals(x, o)))
            .ToArray();

        await movementRepository.AddRangeAsync(newRecords, cancellationToken, true);

        return CommandResult.Success();
    }
}

public class UploadMovementsFileCommand : ICommand
{
    public UploadMovementsFileCommand(IFormFile file, Guid appModuleId, Guid bankId, DateTimeKind dateKind)
    {
        File = file;
        AppModuleId = appModuleId;
        BankId = bankId;
        DateKind = dateKind;
    }

    public IFormFile File { get; set; }
    public Guid AppModuleId { get; set; }
    public Guid BankId { get; set; }
    public DateTimeKind DateKind { get; set; }
}
