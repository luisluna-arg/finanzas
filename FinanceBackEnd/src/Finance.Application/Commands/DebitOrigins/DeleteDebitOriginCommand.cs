using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Debits;

namespace Finance.Application.Commands.DebitOrigins;

public class DeleteDebitOriginCommand : DeleteEntityCommand<Guid>;

public sealed class DeleteDebitOriginCommandHandler(IRepository<DebitOrigin, Guid> repository)
    : DeleteEntityCommandHandler<DebitOrigin, Guid, DeleteDebitOriginCommand, DeleteDebitOriginCommandValidator>(repository);

public sealed class DeleteDebitOriginCommandValidator()
    : DeleteEntityCommandValidator<DeleteDebitOriginCommand, Guid>();
