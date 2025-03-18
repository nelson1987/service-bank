using AutoFixture;
using FluentValidation.TestHelper;
using Junta.Web.Controllers;

namespace Junta.UnitTests;

public class CreateProductHandlerUnitTests : UnitTests
{
    //private readonly Mock<ICreateProductCommandHandler> _createProductCommandHandler;
    private readonly CreateProductCommand _command;
    private readonly CreateProductHandler _sut;

    public CreateProductHandlerUnitTests()
    {
        //_createProductCommandHandler = Fixture.Freeze<Mock<ICreateProductCommandHandler>>();
        _command = Fixture.Create<CreateProductCommand>();
        _sut = Fixture.Create<CreateProductHandler>();
    }

    [Fact]
    public async Task Post_ContaCorrente_Valido()
    {
        var result = Fixture.Create<CreateProductResponse>();
        // _createProductCommandHandler
        //     .Setup(x => x.HandleAsync(_command))
        //     .ReturnsAsync(Result.Success(result));
        await _sut.HandleAsync(_command);
    }
}
public class CreateProductCommandUnitTests : UnitTests
{
    private readonly CreateProductCommand _command;
    private readonly CreateProductValidator _sut;

    public CreateProductCommandUnitTests()
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