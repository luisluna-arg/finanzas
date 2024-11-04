using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.Incomes;

public class CreateIncomeCommandHandler : BaseResponseHandler<CreateIncomeCommand, Income>
{
    private readonly IRepository<Bank, Guid> bankRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;
    private readonly IRepository<Income, Guid> incomeRepository;

    public CreateIncomeCommandHandler(
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

    public override async Task<Income> Handle(CreateIncomeCommand command, CancellationToken cancellationToken)
    {
        Bank? bank = await bankRepository.GetByIdAsync(command.BankId, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        Currency? currency = await currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var newIncome = new Income()
        {
            Bank = bank,
            Currency = currency,
            Amount = command.Amount,
            TimeStamp = command.TimeStamp,
            CreatedAt = DateTime.UtcNow,
            Deactivated = false
        };

        await this.incomeRepository.AddAsync(newIncome, cancellationToken);

        return await Task.FromResult(newIncome);
    }
}

public class CreateIncomeCommand : IRequest<Income>
{
    public virtual Guid BankId { get; set; }

    public virtual Guid CurrencyId { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public decimal Amount { get; set; }
}
