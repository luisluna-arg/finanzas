using FinanceApi.Application.Base.Handlers;
using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.Funds;

public class UpdateFundCommandHandler : BaseResponseHandler<UpdateFundCommand, Fund>
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

    public override async Task<Fund> Handle(UpdateFundCommand command, CancellationToken cancellationToken)
    {
        var fund = await fundRepository.GetById(command.Id);
        if (fund == null) throw new Exception("Fund not found");

        var currency = await currencyRepository.GetById(command.CurrencyId);
        if (currency == null) throw new Exception("Currency not found");

        var bank = await bankRepository.GetById(command.BankId);
        if (bank == null) throw new Exception("Bank not found");

        fund.Currency = currency;
        fund.Bank = bank;
        fund.Amount = command.Amount;
        fund.TimeStamp = command.TimeStamp;

        await fundRepository.Update(fund);

        return await Task.FromResult(fund);
    }
}

public class UpdateFundCommand : IRequest<Fund>
{
    required public Guid Id { get; set; }

    public virtual Guid BankId { get; set; }

    public virtual Guid CurrencyId { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public Money Amount { get; set; }
}
