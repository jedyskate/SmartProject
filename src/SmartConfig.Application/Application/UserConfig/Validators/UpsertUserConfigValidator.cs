using FluentValidation;
using SmartConfig.Application.Application.UserConfig.Commands;

namespace SmartConfig.Application.Application.UserConfig.Validators;

public class UpsertUserConfigValidator : AbstractValidator<UpsertUserConfigCommand>
{
    public UpsertUserConfigValidator()
    {
        RuleFor(obj => obj.Identifier)
            .Must(r => !string.IsNullOrEmpty(r))
            .WithMessage("UserConfig identifier parameter required");

        RuleFor(obj => obj.Name)
            .Must(r => !string.IsNullOrEmpty(r))
            .WithMessage("UserConfig name parameter required");
    }
}