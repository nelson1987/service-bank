using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;

namespace TestProject1.Service;

public record CreateUserCommand(string Nome)
{
    public static implicit operator User(CreateUserCommand source)
    {
        return new User(source.Nome);
    }
};

public record CreateUserResponse(Guid Id, string Nome);

public class CreateUserHandler : BaseHandler<CreateUserResponse>
{
    private readonly IRepository<User> _repository;
    private readonly IPersistence _persistence;
    private readonly IPublisher _publisher;

    public CreateUserHandler(IRepository<User> repository, IPersistence persistence, IPublisher publisher)
    {
        _repository = repository;
        _persistence = persistence;
        _publisher = publisher;
    }

    public async Task<HandlerResponse<CreateUserResponse>> HandleAsync(CreateUserCommand createUserCommand,
        CancellationToken cancellationToken = default)
    {
        User entity = await _repository.AddAsync(createUserCommand, cancellationToken);
        var response = new CreateUserResponse(entity.Id, entity.Nome);
        _ = Task.Run(() =>
        {
            _persistence.Set(response);
            _publisher.Send(response);
        }, cancellationToken);
        return Success(response);
    }
}

public class User
{
    public User(string nome)
    {
        Id = Guid.NewGuid();
        Nome = nome;
    }

    public Guid Id { get; private set; }
    public string Nome { get; private set; }
}

public interface IRepository<TEntity>
{
    Task<User> AddAsync(User entity, CancellationToken cancellationToken = default);
}

public interface IPersistence
{
    void Set(CreateUserResponse createUserCommand);
}

public interface IPublisher
{
    void Send(CreateUserResponse createUserCommand);
}

public class HandlerResponse<T>
{
    public HandlerResponse(T? value)
    {
        Value = value;
    }

    public HandlerResponse(Exception? value)
    {
        Error = value;
        IsValid = false;
    }

    public Exception? Error { get; }
    public T? Value { get; }
    public bool IsValid { get; private set; } = true;
}

public class BaseHandler<T>
{
    protected HandlerResponse<T> Success(T? value)
    {
        return new HandlerResponse<T>(value);
    }

    static HandlerResponse<T> Failure(Exception? error)
    {
        return new HandlerResponse<T>(error);
    }
}

public class BasicEndpointUnitTests
{
    private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization
        {
            ConfigureMembers = true
        });

    private readonly Mock<IRepository<User>> _repositoryMock;
    private readonly Mock<IPersistence> _persistenceMock;
    private readonly Mock<IPublisher> _publisherMock;
    private readonly CreateUserCommand _createUserCommand;
    private readonly CreateUserHandler _createUserHandler;

    public BasicEndpointUnitTests()
    {
        _repositoryMock = _fixture.Freeze<Mock<IRepository<User>>>();
        _persistenceMock = _fixture.Freeze<Mock<IPersistence>>();
        _publisherMock = _fixture.Freeze<Mock<IPublisher>>();
        _createUserCommand = _fixture.Create<CreateUserCommand>();
        _createUserHandler = _fixture.Create<CreateUserHandler>();
        // Repository
        _repositoryMock
            .Setup(x => x.AddAsync(
                It.Is<User>(y => y.Nome == _createUserCommand.Nome)
                , It.IsAny<CancellationToken>()))
            .ReturnsAsync(_createUserCommand);
    }

    [Fact]
    public async Task FirstTest()
    {
        var actualresponse =
            await _createUserHandler.HandleAsync(_createUserCommand, It.IsAny<CancellationToken>());
        actualresponse.Should().BeOfType<HandlerResponse<CreateUserResponse>>();
        actualresponse.Value!.Nome.Should().Be(_createUserCommand.Nome);
    }

    [Fact]
    public async Task FirstTest_RaisedExceptionInRepository()
    {
        _repositoryMock
            .Setup(x => x.AddAsync(
                It.Is<User>(y => y.Nome == _createUserCommand.Nome)
                , It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotImplementedException());
        var actualresponse = async () => await _createUserHandler.HandleAsync(_createUserCommand);
        await actualresponse.Should().ThrowExactlyAsync<NotImplementedException>();
    }

    [Fact]
    public async Task FirstTest_RaisedExceptionInPersistence()
    {
        _persistenceMock
            .Setup(x => x
                .Set(It.Is<CreateUserResponse>(x => x.Nome == _createUserCommand.Nome)))
            .Throws(new NotImplementedException());
        var actualresponse = await _createUserHandler.HandleAsync(_createUserCommand);
        actualresponse.Should().BeOfType<HandlerResponse<CreateUserResponse>>();
        actualresponse.Value!.Nome.Should().Be(_createUserCommand.Nome);
    }

    [Fact]
    public async Task FirstTest_RaisedExceptionInProducer()
    {
        _publisherMock
            .Setup(x => x.Send(It.Is<CreateUserResponse>(x => x.Nome == _createUserCommand.Nome)))
            .Throws(new NotImplementedException());
        var actualresponse = await _createUserHandler.HandleAsync(_createUserCommand);
        actualresponse.Should().BeOfType<HandlerResponse<CreateUserResponse>>();
        actualresponse.Value!.Nome.Should().Be(_createUserCommand.Nome);
    }
}