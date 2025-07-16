using Finance.Application.Base.Handlers;
using Finance.Domain.SpecialTypes;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Commands.Incomes;

public class UpdateIncomeCommandHandler : BaseCommandHandler<UpdateIncomeCommand, Income>
{
    private readonly IRepository<Bank, Guid> _bankRepository;
    private readonly IRepository<Currency, Guid> _currencyRepository;
    private readonly IRepository<Income, Guid> _incomeRepository;

    public UpdateIncomeCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<Income, Guid> incomeRepository)
        : base(db)
    {
        _bankRepository = bankRepository;
        _currencyRepository = currencyRepository;
        _incomeRepository = incomeRepository;
    }

    public override async Task<DataResult<Income>> ExecuteAsync(UpdateIncomeCommand command, CancellationToken cancellationToken)
    {
        var income = await _incomeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (income == null) throw new Exception("Income not found");

        var currency = await _currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var bank = await _bankRepository.GetByIdAsync(command.BankId, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        income.Currency = currency;
        income.Bank = bank;
        income.Amount = command.Amount;
        income.TimeStamp = command.TimeStamp;

        await _incomeRepository.UpdateAsync(income, cancellationToken);

        return DataResult<Income>.Success(income);
    }
}

public class UpdateIncomeCommand : ICommand
{
    required public Guid Id { get; set; }

    public virtual Guid BankId { get; set; }

    public virtual Guid CurrencyId { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public Money Amount { get; set; }
}
