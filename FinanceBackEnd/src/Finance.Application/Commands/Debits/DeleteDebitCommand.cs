using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Commands.Debits;

public class DeleteDebitCommand : DeleteEntityCommand<Guid>;

public sealed class DeleteDebitCommandHandler(IRepository<Debit, Guid> repository)
    : DeleteEntityCommandHandler<Debit, Guid, DeleteDebitCommand, DeleteDebitCommandValidator>(repository);

public sealed class DeleteDebitCommandValidator()
    : DeleteEntityCommandValidator<DeleteDebitCommand, Guid>();
