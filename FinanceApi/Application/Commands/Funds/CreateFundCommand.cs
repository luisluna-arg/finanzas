using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.Funds;

public class CreateFundCommandHandler : BaseResponseHandler<CreateFundCommand, Fund>
{
    private readonly IRepository<Bank, Guid> bankRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;
    private readonly IRepository<Fund, Guid> fundRepository;

    public CreateFundCommandHandler(
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

    public override async Task<Fund> Handle(CreateFundCommand command, CancellationToken cancellationToken)
    {
        Bank? bank = await bankRepository.GetByIdAsync(command.BankId, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        Currency? currency = await currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var newFund = new Fund()
        {
            Bank = bank,
            Currency = currency,
            Amount = command.Amount,
            TimeStamp = command.TimeStamp,
            CreatedAt = DateTime.UtcNow,
            Deactivated = false,
            DailyUse = command.DailyUse ?? false
        };

        await this.fundRepository.AddAsync(newFund, cancellationToken);

        return await Task.FromResult(newFund);
    }
}

public class CreateFundCommand : IRequest<Fund>
{
    public virtual Guid BankId { get; set; }

    public virtual Guid CurrencyId { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public decimal Amount { get; set; }

    public bool? DailyUse { get; set; }
}
