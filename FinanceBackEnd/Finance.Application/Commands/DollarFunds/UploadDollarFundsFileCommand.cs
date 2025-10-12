using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Helpers;
using Finance.Application.Repositories;
using Finance.Application.Repositories.Base;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Commands.DollarFunds;

public class UploadDollarFundsFileCommandHandler : BaseResponselessHandler<UploadDollarFundsFileCommand>
{
    private readonly IAppModuleRepository _appModuleRepository;
    private readonly IRepository<Movement, Guid> _movementRepository;
    private readonly IRepository<Bank, Guid> _bankRepository;
    private readonly FundsExcelHelper _excelHelper;

    public UploadDollarFundsFileCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<Movement, Guid> movementRepository,
        IRepository<Bank, Guid> bankRepository)
        : base(db)
    {
        _appModuleRepository = appModuleRepository;
        _movementRepository = movementRepository;
        _bankRepository = bankRepository;
        _excelHelper = new FundsExcelHelper();
    }

    public override async Task<CommandResult> ExecuteAsync(UploadDollarFundsFileCommand command, CancellationToken cancellationToken)
    {
        var appModule = await _appModuleRepository.GetDollarFundsAsync(cancellationToken);

        var dateKind = command.DateKind;
        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;

        var bank = await _bankRepository.GetByAsync("Id", command.BankId, cancellationToken);
        if (bank == null) throw new Exception($"Bank not found, Id: {command.BankId}");

        var newRecords = _excelHelper.Read(command.File, appModule, bank, dateKind);
        if (newRecords == null || !newRecords.Any()) return CommandResult.Failure("No records found in the uploaded file.");

        var minDate = newRecords.Min(o => o.TimeStamp);
        var maxDate = newRecords.Max(o => o.TimeStamp);

        var timeStampProperty = "TimeStamp";
        var existingRecords = _movementRepository
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

        await _movementRepository.AddRangeAsync(newRecords, cancellationToken, true);

        return CommandResult.Success();
    }
}

public class UploadDollarFundsFileCommand : ICommand
{
    public UploadDollarFundsFileCommand(IFormFile file, Guid bankId, DateTimeKind dateKind)
    {
        this.File = file;
        this.BankId = bankId;
        this.DateKind = dateKind;
    }

    public IFormFile File { get; set; }
    public Guid BankId { get; set; }
    public DateTimeKind DateKind { get; set; }
}
