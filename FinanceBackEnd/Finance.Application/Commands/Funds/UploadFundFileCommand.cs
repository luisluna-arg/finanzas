using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Comparers;
using Finance.Domain.Models;
using Finance.Helpers;
using Finance.Application.Repositories;
using Finance.Application.Repositories.Base;
using Finance.Persistance;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Commands.Funds;

public class UploadFundFileCommandHandler : BaseResponselessHandler<UploadFundFileCommand>
{
    private readonly IAppModuleRepository appModuleRepository;
    private readonly IRepository<Movement, Guid> movementRepository;
    private readonly IRepository<Bank, Guid> bankRepository;
    private readonly FundsExcelHelper excelHelper;

    public UploadFundFileCommandHandler(
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

    public override async Task Handle(UploadFundFileCommand command, CancellationToken cancellationToken)
    {
        var appModule = await appModuleRepository.GetFundsAsync(cancellationToken);

        var dateKind = command.DateKind;
        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;

        var bank = await bankRepository.GetByAsync("Id", command.BankId, cancellationToken);
        if (bank == null) throw new Exception($"Bank not found, Id: {command.BankId}");

        var newRecords = excelHelper.Read(command.File, appModule, bank, dateKind);
        if (newRecords == null || !newRecords.Any()) return;

        var minDate = newRecords.Min(o => o.TimeStamp);
        var maxDate = newRecords.Max(o => o.TimeStamp);

        var timeStampProperty = "TimeStamp";
        var existingRecords = movementRepository
            .FilterBy(timeStampProperty, ExpressionOperator.GreaterThanOrEqual, minDate)
            .FilterBy(timeStampProperty, ExpressionOperator.LessThanOrEqual, maxDate)
            .Where(o => o.AppModule.Id == appModule.Id)
            .ToArray();

        var movementComparer = new MovementComparer();
        newRecords = newRecords
            .Where(o => existingRecords.All(x => movementComparer.Equals(x, o)))
            .ToArray();

        await movementRepository.AddRangeAsync(newRecords, cancellationToken, true);
    }
}

public class UploadFundFileCommand : IRequest
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
