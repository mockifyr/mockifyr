using FluentValidation;

namespace Application.Features.Identity.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        //RuleFor(v => v.Email)
        //    .MaximumLength(200)
        //    .NotEmpty();
    }
}
