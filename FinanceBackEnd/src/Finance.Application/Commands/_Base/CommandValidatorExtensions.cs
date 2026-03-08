using FluentValidation;
using FluentValidation.Results;

namespace Finance.Application.Commands.Base;

public static class CommandValidatorExtensions
{
    public static void ThrowIfNotValid<TCommand>(this TCommand command, AbstractValidator<TCommand> validator)
    {
        var result = validator.Validate(command);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }

    public static bool IsValid<TCommand>(this AbstractValidator<TCommand> validator, TCommand command, out ICollection<ValidationFailure> errors)
    {
        var result = validator.Validate(command);
        errors = new List<ValidationFailure>();
        if (!result.IsValid)
        {
            ((List<ValidationFailure>)errors).AddRange(result.Errors);
        }
        return result.IsValid;
    }
}