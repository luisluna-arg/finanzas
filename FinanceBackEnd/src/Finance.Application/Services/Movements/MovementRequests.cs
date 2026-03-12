using Finance.Domain.SpecialTypes;

namespace Finance.Application.Services.Movements;

public sealed record CreateMovementRequest(
    Guid? AppModuleId,
    Guid? CurrencyId,
    DateTime TimeStamp,
    string Concept1,
    string? Concept2,
    Money Amount,
    Money? Total);

public sealed record UpdateMovementRequest(
    Guid Id,
    DateTime TimeStamp,
    string Concept1,
    string? Concept2,
    Money Amount,
    Money? Total);

public sealed record DeleteMovementRequest(Guid[] Ids);
