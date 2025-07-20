using Finance.Application.Base.Handlers;
using Finance.Domain.Comparers;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Helpers;
using Finance.Application.Repositories;
using Finance.Application.Repositories.Base;
using Finance.Persistance;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Commands.Funds;

public class UploadFundFileCommandHandler : BaseResponselessHandler<UploadFundFileCommand>
{
    private readonly IAppModuleRepository _appModuleRepository;
    private readonly IRepository<Movement, Guid> _movementRepository;
    private readonly IRepository<Bank, Guid> _bankRepository;
    private readonly FundsExcelHelper _excelHelper;

    public UploadFundFileCommandHandler(
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

    public override async Task<CommandResult> ExecuteAsync(UploadFundFileCommand command, CancellationToken cancellationToken)
    {
        var appModule = await _appModuleRepository.GetFundsAsync(cancellationToken);

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

        var movementComparer = new MovementComparer();
        newRecords = newRecords
            .Where(o => existingRecords.All(x => movementComparer.Equals(x, o)))
            .ToArray();

        await _movementRepository.AddRangeAsync(newRecords, cancellationToken, true);

        return CommandResult.Success();
    }
}

public class UploadFundFileCommand : ICommand
{
    public UploadFundFileCommand(IFormFile file, Guid bankId, DateTimeKind dateKind)
    {
        this.File = file;
        this.BankId = bankId;
        this.DateKind = dateKind;
    }

    public IFormFile File { get; set; }
    public Guid BankId { get; set; }
    public DateTimeKind DateKind { get; set; }
}
