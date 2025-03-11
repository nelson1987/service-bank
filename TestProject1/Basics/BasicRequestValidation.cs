using FluentValidation;

namespace TestProject1.Basics;

public class BasicRequestValidation : AbstractValidator<BasicRequest>
{
    public BasicRequestValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(Constants.NotEmptyFirstName);
    }
}