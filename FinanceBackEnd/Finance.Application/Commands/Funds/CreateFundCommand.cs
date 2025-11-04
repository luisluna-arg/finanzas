using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;

namespace Finance.Application.Commands.Funds;

public class CreateFundCommandHandler : BaseCommandHandler<CreateFundCommand, Fund>
{
    private readonly IRepository<Bank, Guid> _bankRepository;
    private readonly IRepository<Currency, Guid> _currencyRepository;
    private readonly IRepository<Fund, Guid> _fundRepository;

    public CreateFundCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<Fund, Guid> fundRepository)
        : base(db)
    {
        _bankRepository = bankRepository;
        _currencyRepository = currencyRepository;
        _fundRepository = fundRepository;
    }

    public override async Task<DataResult<Fund>> ExecuteAsync(CreateFundCommand command, CancellationToken cancellationToken)
    {
        Bank? bank = await _bankRepository.GetByIdAsync(command.BankId, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        Currency? currency = await _currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var newFund = new Fund()
        {
            Bank = bank,
            Currency = currency,
            Amount = command.Amount,
            TimeStamp = command.TimeStamp,
            Deactivated = false,
            DailyUse = command.DailyUse ?? false
        };

        await _fundRepository.AddAsync(newFund, cancellationToken);

        return DataResult<Fund>.Success(newFund);
    }
}

public class CreateFundCommand : ICommand<DataResult<Fund>>
{
    public virtual Guid BankId { get; set; }
    public virtual Guid CurrencyId { get; set; }
    required public DateTime TimeStamp { get; set; }
    required public Money Amount { get; set; }
    public bool? DailyUse { get; set; }
}
