using Finance.Domain.Enums;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Services.Debits;

public sealed record CreateDebitRequest(
    Guid AppModuleId,
    string Origin,
    Money Amount,
    bool Deactivated,
    FrequencyEnum Frequency);

public sealed record UpdateDebitRequest(
    Guid Id,
    Guid AppModuleId,
    string Origin,
    Money Amount,
    bool Deactivated,
    FrequencyEnum Frequency);

public sealed record DeleteDebitRequest(Guid[] Ids);

public sealed record ActivateDebitRequest(Guid[] Ids);

public sealed record DeactivateDebitRequest(Guid[] Ids);
