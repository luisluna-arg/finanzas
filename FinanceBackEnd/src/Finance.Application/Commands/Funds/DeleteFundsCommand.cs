using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Funds;

namespace Finance.Application.Commands.Funds;

public sealed class DeleteFundsCommand() : DeleteEntityCommand<Guid>();

public sealed class DeleteFundsCommandHandler(IRepository<Fund, Guid> repository)
    : DeleteEntityCommandHandler<Fund, Guid, DeleteFundsCommand, DeleteFundsCommandValidator>(repository);

public sealed class DeleteFundsCommandValidator()
    : DeleteEntityCommandValidator<DeleteFundsCommand, Guid>();
