using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;
using MediatR;

namespace FinanceApi.Application.Commands.Currencies;

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
        => await currencyRepository.Delete(command.Id);
}

public class DeleteCurrencyCommand : IRequest
{
    [Required]
    public Guid Id { get; set; }
}
