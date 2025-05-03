using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

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

    public override async Task Handle(DeleteCurrencyCommand command, CancellationToken cancellationToken)
        => await currencyRepository.DeleteAsync(command.Id, cancellationToken);
}

public class DeleteCurrencyCommand : IRequest
{
    [Required]
    public Guid Id { get; set; }
}
