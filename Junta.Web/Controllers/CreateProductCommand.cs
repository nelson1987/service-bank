using FluentValidation;

namespace Junta.Web.Controllers;

public record CreateProductCommand(string Name, string Description, decimal Price, int Stock);

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull();
        RuleFor(x => x.Description).NotEmpty().NotNull();
        RuleFor(x => x.Price).NotEmpty().NotNull().GreaterThan(0);
        RuleFor(x => x.Stock).NotEmpty().NotNull().GreaterThan(0);
    }
}