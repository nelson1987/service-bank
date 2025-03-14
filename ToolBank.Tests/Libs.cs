namespace ToolBank.Tests.Libs;

#region BaseHandler - Lib de Handler-UseCases

public record CreateResultadoCommand(string Nome) : ICommand<CreateResultadoResponse>;

public record CreateResultadoResponse(string Id, string Nome);

public interface ICommand<out TResult>
{
}

public interface IBaseHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : class
{
    Task<HandlerResult<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public abstract class BaseHandler<TCommand, TResponse> : IBaseHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : class
{
    public abstract Task<HandlerResult<TResponse>>
        HandleAsync(TCommand command, CancellationToken cancellationToken = default);

    protected HandlerResult<TResponse> Ok(TResponse response)
    {
        return HandlerResult.Success(response);
    }

    protected HandlerResult<TResponse> Failure()
    {
        return HandlerResult.Fail<TResponse>();
    }

    protected HandlerResult<TResponse> NotFound(Guid id)
    {
        return HandlerResult.NotFound<TResponse>(id);
    }
}

public class CreateResultadoHandlerUnitTests
{
    [Fact]
    public async Task When_CreateUserHandle_Then_WithoutResponse_Should_Successed()
    {
        CreateResultadoHandler handler = new CreateResultadoHandler();
        CreateResultadoCommand command = new
            CreateResultadoCommand("Nome do Usuario");
        var result = await handler.HandleAsync(command);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.IsType<HandlerResult<CreateResultadoResponse>>(result);
        Assert.IsType<CreateResultadoResponse>(result.Value);
    }

    [Fact]
    public async Task When_CreateUserHandle_Then_WithoutResponse_Should_Failed()
    {
        CreateResultadoHandler handler = new CreateResultadoHandler();
        CreateResultadoCommand command = new
            CreateResultadoCommand("Fail");
        var result = await handler.HandleAsync(command);
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.IsType<HandlerResult<CreateResultadoResponse>>(result);
        Assert.Null(result.Value);
    }
    [Fact]
    public async Task When_CreateUserHandle_Then_WithoutResponse_Should_NotFound()
    {
        CreateResultadoHandler handler = new CreateResultadoHandler();
        CreateResultadoCommand command = new
            CreateResultadoCommand("NotFound");
        var result = await handler.HandleAsync(command);
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.IsType<HandlerResult<CreateResultadoResponse>>(result);
        Assert.Null(result.Value);
        Assert.NotEmpty(result.Errors);
    }
}

#region Handler de Exemplo

public class CreateResultadoHandler : BaseHandler<CreateResultadoCommand, CreateResultadoResponse>
{
    public override Task<HandlerResult<CreateResultadoResponse>> HandleAsync(CreateResultadoCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Nome.Equals("Fail"))
            return Task.FromResult(Failure());
        if (command.Nome.Equals("NotFound"))
            return Task.FromResult(NotFound(Guid.NewGuid()));
        return Task.FromResult(Ok(new CreateResultadoResponse("Id", "Nome do Usuario")));
    }
}

#endregion

#endregion

#region HandlerResult - Lib de Resultados

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

public static class ConstantsErrors
{
    public static readonly Error SameUser = new Error(
        "Followers.SameUser", "Can't follow yourself");

    public static Error NotFound(Guid id) => new Error(
        "ContaDebitante.NotFound", $"The follower with Id '{id}' was not found");
}

public interface IResultadoBase
{
    bool IsFailed { get; }
    bool IsSuccess { get; }
    List<Error> Errors { get; }
    object? Value { get; }
}

public abstract class ResultadoBase : IResultadoBase
{
    public bool IsFailed => !IsSuccess;
    public bool IsSuccess { get; set; }
    public List<Error> Errors { get; }
    public object? Value { get; }

    public ResultadoBase()
    {
        Errors = new List<Error>();
    }
}

public abstract class ResultadoBase<TResult> : ResultadoBase
    where TResult : ResultadoBase<TResult>
{
}

public class HandlerResult<TValue> : ResultadoBase<HandlerResult<TValue>>
{
    public TValue Value { get; set; }
}

public partial class HandlerResult : ResultadoBase<HandlerResult>
{
}

public partial class HandlerResult
{
    public static HandlerResult Success() => new HandlerResult() { IsSuccess = true };
    public static HandlerResult Fail() => new HandlerResult() { IsSuccess = false };

    public static HandlerResult<TValue> Success<TValue>(TValue value) =>
        new HandlerResult<TValue>() { IsSuccess = true, Value = value };

    public static HandlerResult<TValue> Fail<TValue>() =>
        new HandlerResult<TValue>() { IsSuccess = false };

    public static HandlerResult<TValue> NotFound<TValue>(Guid id) =>
        new HandlerResult<TValue>() { IsSuccess = false, Errors = { ConstantsErrors.NotFound(id) } };
}

public class HandlerResultUnitTests
{
    [Fact]
    public void When_CreateUserHandle_Then_WithoutResponse_Should_Successed()
    {
        Handler handler = new Handler();
        CreateResultadoCommand command = new
            CreateResultadoCommand("Nome do Usuario");
        var result = handler.CreateUserHandle(command);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.IsType<HandlerResult>(result);
    }

    [Fact]
    public void When_CreateUserHandle_Then_WithoutResponse_Should_Failed()
    {
        Handler handler = new Handler();
        CreateResultadoCommand command = new
            CreateResultadoCommand("Fail");
        var result = handler.CreateUserHandle(command);
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.IsType<HandlerResult>(result);
    }

    [Fact]
    public void When_CreateUserHandle_Then_WithResponse_Should_Successed()
    {
        Handler handler = new Handler();
        CreateResultadoCommand command = new
            CreateResultadoCommand("Nome do Usuario");
        var result = handler.CreateUserHandleWithCare(command);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.IsType<HandlerResult<CreateResultadoResponse>>(result);
        Assert.IsType<CreateResultadoResponse>(result.Value);
    }

    [Fact]
    public void When_CreateUserHandle_Then_WithResponse_Should_Failed()
    {
        Handler handler = new Handler();
        CreateResultadoCommand command = new
            CreateResultadoCommand("Fail");
        var result = handler.CreateUserHandleWithCare(command);
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.IsType<HandlerResult<CreateResultadoResponse>>(result);
        Assert.Null(result.Value);
    }
}

#region Handler de Exemplo

public class Handler
{
    public HandlerResult CreateUserHandle(CreateResultadoCommand command)
    {
        if (command.Nome.Equals("Fail"))
            return HandlerResult.Fail();
        return HandlerResult.Success();
    }

    public HandlerResult<CreateResultadoResponse> CreateUserHandleWithCare(CreateResultadoCommand command)
    {
        if (command.Nome.Equals("Fail"))
            return HandlerResult.Fail<CreateResultadoResponse>();
        return HandlerResult.Success(new CreateResultadoResponse("Id", "Nome do Usuario"));
    }
}

#endregion

#endregion