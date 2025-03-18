using AutoFixture;
using FluentValidation.TestHelper;
using Junta.Domain;
using Junta.Web.Controllers;

namespace Junta.UnitTests;

public class CreateProductValidatorUnitTests : UnitTests
{
    private readonly CreateProductCommand _command;
    private readonly CreateProductValidator _sut;

    public CreateProductValidatorUnitTests()
    {
        _command = Fixture.Create<CreateProductCommand>();
        _sut = Fixture.Create<CreateProductValidator>();
    }

    [Fact]
    public void Given_CriarTransferenciaIsValid_When_Validation_Then_ResponseSuccess()
        => _sut
            .TestValidate(_command)
            .ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void Given_CriarTransferenciaIsValid_When_ValorIsGreaterThanZero_Then_ResponseSuccess()
        => _sut
            .TestValidate(_command with { Price  = 0.01M })
            .ShouldNotHaveValidationErrorFor(x => x.Price);
}