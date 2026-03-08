using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using Finance.Application.Commands.Base;
using Finance.Application.Commands.Funds.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Funds;
using Finance.Persistence;
using FluentValidation;

namespace Finance.Application.Commands.Funds;

public class UpdateFundCommandHandler(
    FinanceDbContext db,
    IRepository<Fund, Guid> fundRepository,
    IRepository<Bank, Guid> bankRepository,
    IRepository<Currency, Guid> currencyRepository) : BaseCommandHandler<UpdateFundCommand, Fund>(db)
{
    private readonly IRepository<Fund, Guid> _fundRepository = fundRepository;
    private readonly IRepository<Bank, Guid> _bankRepository = bankRepository;
    private readonly IRepository<Currency, Guid> _currencyRepository = currencyRepository;

    public override async Task<DataResult<Fund>> ExecuteAsync(UpdateFundCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(new UpdateFundCommandValidator());

        var fund = await _fundRepository.GetByIdAsync(command.Id, cancellationToken);
        if (fund == null) throw new Exception("Fund not found");

        var currency = await _currencyRepository.GetByIdAsync(command.CurrencyId, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        var bank = await _bankRepository.GetByIdAsync(command.BankId, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        fund.Currency = currency;
        fund.Bank = bank;
        fund.Amount = command.Amount;
        fund.TimeStamp = command.TimeStamp;
        fund.DailyUse = command.DailyUse ?? false;

        await _fundRepository.UpdateAsync(fund, cancellationToken);

        return DataResult<Fund>.Success(fund);
    }
}

public class UpdateFundCommand : UpsertFundBaseCommand
{
    [Required]
    public Guid Id { get; set; }
}

public class UpdateFundCommandValidator : UpsertFundBaseCommandValidator<UpdateFundCommand>
{
    public UpdateFundCommandValidator() : base()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Fund Id is required.");
    }
}
