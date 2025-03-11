using AutoFixture;
using FluentAssertions;
using Moq;

namespace TestProject1.Basics;

public class BasicEndpointUnitTests : BasicUnitTests
{
    private readonly BasicEndpoint _handler;
    private readonly Mock<IRepository<Entity>> _repository;
    private readonly BasicRequest _request;

    public BasicEndpointUnitTests()
    {
        _repository = Fixture.Freeze<Mock<IRepository<Entity>>>();
        _request = Fixture.Create<BasicRequest>();
        _handler = Fixture.Create<BasicEndpoint>();
    }

    [Fact]
    public void WithSuccess()
    {
        var result = _handler.Post(_request);
        result.Should().BeOfType<BasicResponse>();
        result.Id.Should().NotBeEmpty();
        _repository.Verify(
            l => l.Save(It.Is<Entity>(x => x.Name == _request.Name)), // Verifica se a mensagem foi passada
            Times.Once // Garante que o método foi chamado uma vez
        );
    }

    [Fact]
    public void WithErrors()
    {
        _repository.Setup(x => x.Save(It.IsAny<Entity>())).Throws(new Exception());
        var result = () => _handler.Post(_request);
        result.Should().Throw<Exception>();
        _repository.Verify(
            l => l.Save(It.Is<Entity>(x => x.Name == _request.Name)),
            Times.Once
        );
    }
}