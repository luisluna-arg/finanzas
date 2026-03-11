
namespace Finance.Application.Services.DebitOrigins;

public sealed record CreateDebitOriginRequest(Guid AppModuleId, string Name, bool Deactivated);

public sealed record UpdateDebitOriginRequest(Guid Id, Guid AppModuleId, string Name, bool Deactivated);

public sealed record DeleteDebitOriginRequest(Guid[] Ids);
