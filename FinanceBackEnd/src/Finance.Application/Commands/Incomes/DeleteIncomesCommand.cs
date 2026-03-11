using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Incomes;

namespace Finance.Application.Commands.Incomes;

public sealed class DeleteIncomesCommand : DeleteEntityCommand<Guid>;

public sealed class DeleteIncomesCommandHandler(IRepository<Income, Guid> repository)
    : DeleteEntityCommandHandler<Income, Guid, DeleteIncomesCommand, DeleteIncomesCommandValidator>(repository);

public sealed class DeleteIncomesCommandValidator()
    : DeleteEntityCommandValidator<DeleteIncomesCommand, Guid>();
