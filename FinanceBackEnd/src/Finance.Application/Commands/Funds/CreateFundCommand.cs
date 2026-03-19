using CQRSDispatch;
using Finance.Application.Commands.Base;
using Finance.Application.Commands.Funds.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Funds;
using Finance.Persistence;

namespace Finance.Application.Commands.Funds;

public class CreateFundCommand : UpsertFundBaseCommand;

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
        command.ThrowIfNotValid(new CreateFundCommandValidator());

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

public class CreateFundCommandValidator : UpsertFundBaseCommandValidator<CreateFundCommand>;
