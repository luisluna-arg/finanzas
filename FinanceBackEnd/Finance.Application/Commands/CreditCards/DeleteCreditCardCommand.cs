using Finance.Domain.Models;
using Finance.Application.Services;
using MediatR;

namespace Finance.Application.Commands.CreditCards;

public class DeleteCreditCardCommandHandler : IRequestHandler<DeleteCreditCardCommand>
{
    private readonly IEntityService<CreditCard, Guid> service;

    public DeleteCreditCardCommandHandler(
        IEntityService<CreditCard, Guid> repository)
    {
        this.service = repository;
    }

    public async Task Handle(DeleteCreditCardCommand request, CancellationToken cancellationToken)
        => await service.DeleteAsync(request.Ids, cancellationToken);
}

public class DeleteCreditCardCommand : IRequest
{
    public Guid[] Ids { get; set; } = [];
}
