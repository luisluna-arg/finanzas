using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;

namespace Finance.Application.Commands.Funds;

public class UpdateFundCommandHandler : BaseCommandHandler<UpdateFundCommand, Fund>
{
    private readonly IRepository<Bank, Guid> bankRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;
    private readonly IRepository<Fund, Guid> fundRepository;

    public UpdateFundCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<Fund, Guid> fundRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
        this.currencyRepository = currencyRepository;
        this.fundRepository = fundRepository;
    }

    public override async Task<DataResult<Fund>> ExecuteAsync(UpdateFundCommand command, CancellationToken cancellationToken)
    {
        var fund = await fundRepository.GetByIdAsync(command.Id, cancellationToken);
        if (fund == null) throw new Exception("Fund not found");

        var currency = await currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var bank = await bankRepository.GetByIdAsync(command.BankId, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        fund.Currency = currency;
        fund.Bank = bank;
        fund.Amount = command.Amount;
        fund.TimeStamp = command.TimeStamp;
        fund.DailyUse = command.DailyUse ?? false;

        await fundRepository.UpdateAsync(fund, cancellationToken);

        return DataResult<Fund>.Success(fund);
    }
}

public class UpdateFundCommand : ICommand<DataResult<Fund>>
{
    required public Guid Id { get; set; }
    public virtual Guid BankId { get; set; }
    public virtual Guid CurrencyId { get; set; }
    required public DateTime TimeStamp { get; set; }
    required public Money Amount { get; set; }
    public bool? DailyUse { get; set; }
}
