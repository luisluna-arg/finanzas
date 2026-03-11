using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Legacy.Commands.Currencies;

public sealed class DeleteCurrencyCommandHandler : BaseResponselessHandler<DeleteCurrencyCommand>
{
    private readonly IRepository<Currency, Guid> currencyRepository;

    public DeleteCurrencyCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task<CommandResult> ExecuteAsync(DeleteCurrencyCommand command, CancellationToken cancellationToken)
    {
        await currencyRepository.DeleteAsync(command.Id, cancellationToken);
        return CommandResult.Success();
    }
}

public sealed class DeleteCurrencyCommand : ICommand
{
    [Required]
    public Guid Id { get; set; }
}
