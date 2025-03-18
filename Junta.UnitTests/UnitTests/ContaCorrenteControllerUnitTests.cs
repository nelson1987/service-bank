using AutoFixture;
using Junta.Web.Controllers;
using Moq;

namespace Junta.UnitTests;

public class ContaCorrenteControllerUnitTests : UnitTests
{
    private readonly CreateProductCommand _command;
    private readonly Mock<ICreateProductCommandHandler> _createProductCommandHandler;
    private readonly ContaCorrenteController _sut;

    public ContaCorrenteControllerUnitTests()
    {
        _createProductCommandHandler = Fixture.Freeze<Mock<ICreateProductCommandHandler>>();
        _command = Fixture.Create<CreateProductCommand>();
        _sut = Fixture.Build<ContaCorrenteController>()
            .OmitAutoProperties()
            .Create();
    }

    [Fact]
    public async Task Post_ContaCorrente_Valido()
    {
        var result = Fixture.Create<CreateProductResponse>();
        _createProductCommandHandler
            .Setup(x => x.HandleAsync(_command))
            .ReturnsAsync(Result.Success(result));
        await _sut.Post(_createProductCommandHandler.Object, _command);
    }
}