using AutoFixture;
using Junta.Domain;
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