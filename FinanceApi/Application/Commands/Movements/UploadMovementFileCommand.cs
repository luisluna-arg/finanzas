using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Helpers;
using FinanceApi.Infrastructure.Repositories;
using FinanceApi.Infrastructure.Repositories.Base;
using MediatR;

namespace FinanceApi.Application.Commands.Movements;

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

    public override async Task Handle(UploadMovementsFileCommand command, CancellationToken cancellationToken)
    {
        var appModule = await appModuleRepository.GetBy("Id", command.AppModuleId);
        if (appModule == null) throw new Exception($"App Module not found, Id: {command.AppModuleId}");

        var bank = await bankRepository.GetBy("Id", command.BankId);
        if (bank == null) throw new Exception($"Bank not found, Id: {command.BankId}");

        var dateKind = command.DateKind;
        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;

        var newRecords = excelHelper.Read(command.File, appModule, bank, dateKind);
        if (newRecords == null || !newRecords.Any()) return;

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

        await movementRepository.AddRange(newRecords, true);
    }
}

public class UploadMovementsFileCommand : IRequest
{
    public UploadMovementsFileCommand(IFormFile file, Guid appModuleId, Guid bankId, DateTimeKind dateKind)
    {
        this.File = file;
        this.AppModuleId = appModuleId;
        this.BankId = bankId;
        this.DateKind = dateKind;
    }

    public IFormFile File { get; set; }
    public Guid AppModuleId { get; set; }
    public Guid BankId { get; set; }
    public DateTimeKind DateKind { get; set; }
}
