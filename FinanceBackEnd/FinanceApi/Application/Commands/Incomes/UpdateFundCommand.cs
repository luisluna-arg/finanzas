using FinanceApi.Application.Base.Handlers;
using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.Incomes;

public class UpdateIncomeCommandHandler : BaseResponseHandler<UpdateIncomeCommand, Income>
{
    private readonly IRepository<Bank, Guid> bankRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;
    private readonly IRepository<Income, Guid> incomeRepository;

    public UpdateIncomeCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<Income, Guid> incomeRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
        this.currencyRepository = currencyRepository;
        this.incomeRepository = incomeRepository;
    }

    public override async Task<Income> Handle(UpdateIncomeCommand command, CancellationToken cancellationToken)
    {
        var income = await incomeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (income == null) throw new Exception("Income not found");

        var currency = await currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var bank = await bankRepository.GetByIdAsync(command.BankId, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        income.Currency = currency;
        income.Bank = bank;
        income.Amount = command.Amount;
        income.TimeStamp = command.TimeStamp;

        await incomeRepository.UpdateAsync(income, cancellationToken);

        return await Task.FromResult(income);
    }
}

public class UpdateIncomeCommand : IRequest<Income>
{
    required public Guid Id { get; set; }

    public virtual Guid BankId { get; set; }

    public virtual Guid CurrencyId { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public Money Amount { get; set; }
}
