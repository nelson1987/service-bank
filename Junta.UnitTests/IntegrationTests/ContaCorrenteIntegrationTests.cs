using AutoFixture;
using FluentAssertions;
using Junta.Domain;

namespace Junta.UnitTests;

public class ContaCorrenteIntegrationTests : IntegrationTests
{
    private readonly CreateProductCommand _command;

    public ContaCorrenteIntegrationTests()
    {
        _command = Fixture.Create<CreateProductCommand>();
    }

    [Fact]
    public async Task Post_ContaCorrente_Valido()
    {
        // Act
        var response = await Client.PostAsync(Constants.UriPostContacorrente, SetContent(_command));
        // Assert
        response.EnsureSuccessStatusCode();
        var result = GetContent<CreateProductResponse>(response);
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.Name.Should().Be(_command.Name);
        result.Description.Should().Be(_command.Description);
        result.Price.Should().Be(_command.Price);
        result.Stock.Should().Be(_command.Stock);
    }
}

public class ProductRepositoryIntegrationTests : IntegrationTests
{
    private readonly  Product _command;

    public ProductRepositoryIntegrationTests()
    {
        _command = Fixture.Create<Product>();
    }

    [Fact]
    public async Task Post_ContaCorrente_Valido()
    {
        // Act
        await ProductRepository.AddAsync(_command);
        var result = await ProductRepository.GetByIdAsync(_command.Id);
        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.Name.Should().Be(_command.Name);
        result.Description.Should().Be(_command.Description);
        result.Price.Should().Be(_command.Price);
        result.Stock.Should().Be(_command.Stock);
    }
}