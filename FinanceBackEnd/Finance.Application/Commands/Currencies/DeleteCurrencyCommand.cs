using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Commands.Currencies;

public class DeleteCurrencyCommandHandler : BaseResponselessHandler<DeleteCurrencyCommand>
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

public class DeleteCurrencyCommand : ICommand
{
    [Required]
    public Guid Id { get; set; }
}
