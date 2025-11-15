using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Application.Repositories.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models.Debits;
using Finance.Helpers.ExcelHelper;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.Debits;

public class UploadDebitsFileCommandHandler : BaseResponselessHandler<UploadDebitsFileCommand>
{
    private readonly IAppModuleRepository _appModuleRepository;
    private readonly IRepository<Debit, Guid> _repository;
    private readonly IRepository<DebitOrigin, Guid> _originRepository;
    private readonly DebitsExcelHelper _excelHelper;

    public UploadDebitsFileCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<Debit, Guid> debitRepository,
        IRepository<DebitOrigin, Guid> debitOriginRepository)
        : base(db)
    {
        _appModuleRepository = appModuleRepository;
        _originRepository = debitOriginRepository;
        _repository = debitRepository;
        _excelHelper = new DebitsExcelHelper();
    }

    public override async Task<CommandResult> ExecuteAsync(UploadDebitsFileCommand command, CancellationToken cancellationToken)
    {
        var appModule = await _appModuleRepository.GetByIdAsync(command.AppModuleId, cancellationToken);
        if (appModule == null) throw new Exception($"App module not found, Id: {command.AppModuleId}");

        var dateKind = command.DateKind;
        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;

        var newRecords = _excelHelper.Read(command.File, appModule, dateKind);
        if (newRecords == null || !newRecords.Any()) return CommandResult.Failure("No records found in the uploaded file.");

        var minDate = newRecords.Min(o => o.TimeStamp);
        var maxDate = newRecords.Max(o => o.TimeStamp);

        var records = await _repository.GetDbSet()
                .Include(o => o.Origin)
                .ToArrayAsync(cancellationToken);

        var origins = new Dictionary<string, DebitOrigin>();

        foreach (var record in newRecords)
        {
            record.Frequency = command.Frequency;

            var existingRecord = records
                .FirstOrDefault(x => x.Origin.Name.Equals(record.Origin.Name, StringComparison.InvariantCultureIgnoreCase) &&
                    x.TimeStamp == record.TimeStamp && x.Frequency == record.Frequency);
            if (existingRecord != null) continue;

            var origin = origins.ContainsKey(record.Origin.Name) ?
                origins[record.Origin.Name] :
                await _originRepository.GetByAsync("Name", record.Origin.Name, cancellationToken);

            if (origin != null)
            {
                record.Origin = origin;
            }
            else
            {
                origins.Add(record.Origin.Name, record.Origin);
            }

            await _repository.AddAsync(record, cancellationToken, false);
        }

        var timeStampProperty = "TimeStamp";
        var existingRecords = _repository
            .FilterBy(timeStampProperty, ExpressionOperator.GreaterThanOrEqual, minDate)
            .FilterBy(timeStampProperty, ExpressionOperator.LessThanOrEqual, maxDate)
            .Include(o => o.Origin)
            .Where(o => o.Origin.AppModuleId == appModule.Id && o.Frequency == command.Frequency)
            .ToArray();

        newRecords = newRecords
            .Where(o => existingRecords.All(x =>
                x.Origin.AppModuleId != o.Origin.AppModuleId ||
                x.OriginId != o.OriginId ||
                x.TimeStamp != o.TimeStamp ||
                x.Amount != o.Amount))
            .ToArray();

        await _repository.AddRangeAsync(newRecords, cancellationToken, true);

        return CommandResult.Success();
    }
}

public class UploadDebitsFileCommand : ICommand
{
    public UploadDebitsFileCommand(IFormFile file, string appModuleId, DateTimeKind dateKind, FrequencyEnum frequency)
    {
        this.File = file;
        this.AppModuleId = new Guid(appModuleId);
        this.DateKind = dateKind;
        this.Frequency = frequency;
    }

    public IFormFile File { get; set; }
    public Guid AppModuleId { get; set; }
    public DateTimeKind DateKind { get; set; }
    public FrequencyEnum Frequency { get; set; }
}
