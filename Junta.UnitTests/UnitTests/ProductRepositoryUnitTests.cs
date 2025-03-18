using Junta.Domain;
using Junta.Web.Controllers;

namespace Junta.UnitTests;

public class ProductRepositoryUnitTests : UnitTests
{
    //private readonly Mock<ICreateProductCommandHandler> _createProductCommandHandler;
    // private readonly Product _produto;
    private readonly ProductRepository _sut;

    public ProductRepositoryUnitTests()
    {
        //_createProductCommandHandler = Fixture.Freeze<Mock<ICreateProductCommandHandler>>();
        //_produto = Fixture.Create<Product>();
        //_sut = Fixture.Create<ProductRepository>();
    }

    [Fact]
    public void Post_ContaCorrente_Valido()
    {
        // _sut = Fixture.Create<ProductRepository>();
        // var _sut = Fixture.Create<ProductRepository>();
        // _sut = new ProductRepository();

        // _createProductCommandHandler
        //     .Setup(x => x.HandleAsync(_command))
        //     .ReturnsAsync(Result.Success(result));
        // await _sut.AddAsync(_produto);
    }
}