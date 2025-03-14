using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using FluentAssertions;

namespace ToolBank.Tests.ResultTests;

public static class FollowerErrors
{
    public static readonly Error SameUser = new Error(
        "Followers.SameUser", "Can't follow yourself");

    public static readonly Error NonPublicProfile = new Error(
        "Followers.NonPublicProfile", "Can't follow non-public profiles");

    public static readonly Error AlreadyFollowing = new Error(
        "Followers.AlreadyFollowing", "Already following");

    public static Error NotFound(Guid id) => new Error(
        "Followers.NotFound", $"The follower with Id '{id}' was not found");

    public static readonly Error RequiredName = new Error(
        "Name", "Name is required");
}

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

public interface IResultBase
{
    bool IsFailed { get; }
    bool IsSuccess { get; }
    List<Error> Errors { get; }
}

public abstract class ResultBase : IResultBase
{
    public bool IsFailed => !IsSuccess;
    public bool IsSuccess { get; set; }
    public List<Error> Errors { get; }

    public ResultBase()
    {
        Errors = new List<Error>();
    }
}

public abstract class ResultBase<TResult> : ResultBase 
    where TResult : ResultBase<TResult>
{
    public ResultBase()
    {
    }
}

public interface IResult<out TResult> : IResultBase
{
}

public class Result<TValue> : ResultBase<Result<TValue>>, IResult<TValue>
{
    public Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public Result(bool isSuccess, TValue value) : this(isSuccess)
    {
        IsSuccess = isSuccess;
        Value = value;
    }

    public Result(Error error) : this(false)
    {
        Errors.Add(error);
    }

    public TValue Value { get; set; }
}

public partial class Result : ResultBase<Result>
{
    public Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public Result(Error error, bool isSuccess = false) : this(isSuccess)
    {
        Errors.Add(error);
    }

    public Result(List<Error> errors, bool isSuccess = false) : this(isSuccess)
    {
        Errors.AddRange(errors);
    }
}

public partial class Result
{
    public static Result Ok() => new Result(true);
    public static Result<TValue> Ok<TValue>(TValue value) => new Result<TValue>(true, value);
    public static Result Fail() => new Result(false);
    public static Result<TValue> Fail<TValue>() => new Result<TValue>(false);
    public static Result Fail(Error error) => new Result(error);
    public static Result<TValue> Fail<TValue>(Error error) => new Result<TValue>(error);
    public static Result Fail(List<Error> errors) => new Result(errors);
    public static Result<TValue> Fail<TValue>(TValue value) => new Result<TValue>(false, value);
}

public record Response(string Nome);

public class Handler
{
    public Result Handle(string query)
    {
        return Result.Ok();
    }

    public Result<Response> HandleWithCare(string query)
    {
        return Result.Ok(new Response(query));
    }

    public Result HandleWithFail()
    {
        return Result.Fail();
    }

    public Result<Response> HandleWithCareWithFail(string query)
    {
        return Result.Fail(new Response(query));
    }

    public Result HandleWithCareWithFailAndError()
    {
        return Result.Fail(new Error("Code", "Description"));
    }

    public Result HandleWithCareWithFailAndErrorList()
    {
        return Result.Fail([new Error("Code", "Description")]);
    }
}

public class HandlerUnitTests
{
    private Handler _handler;

    public HandlerUnitTests()
    {
        _handler = new Handler();
    }

    [Fact]
    public void Handle()
    {
        var handle = _handler.Handle("");
        handle.Should().BeOfType<Result>();
        handle.IsSuccess.Should().BeTrue();
        handle.IsFailed.Should().BeFalse();
        handle.Errors.Should().BeEmpty();
    }

    [Fact]
    public void HandleWithCare()
    {
        var handle = _handler.HandleWithCare("teste-minimo");
        handle.Should().BeOfType<Result<Response>>();
        handle.IsSuccess.Should().BeTrue();
        handle.IsFailed.Should().BeFalse();
        handle.Errors.Should().BeEmpty();
        handle.Value.Should().BeOfType<Response>();
        handle.Value.Nome.Should().Be("teste-minimo");
    }

    [Fact]
    public void HandleWithFail()
    {
        var handle = _handler.HandleWithFail();
        handle.Should().BeOfType<Result>();
        handle.IsSuccess.Should().BeFalse();
        handle.IsFailed.Should().BeTrue();
        handle.Errors.Should().BeEmpty();
    }

    [Fact]
    public void HandleWithCareWithFail()
    {
        var handle = _handler.HandleWithCareWithFail("teste-minimo");
        handle.Should().BeOfType<Result<Response>>();
        handle.IsSuccess.Should().BeFalse();
        handle.IsFailed.Should().BeTrue();
        handle.Errors.Should().BeEmpty();
        handle.Value.Should().BeOfType<Response>();
        handle.Value.Nome.Should().Be("teste-minimo");
    }

    [Fact]
    public void HandleWithCareWithFailAndError()
    {
        var handle = _handler.HandleWithCareWithFailAndError();
        handle.Should().BeOfType<Result>();
        handle.IsSuccess.Should().BeFalse();
        handle.IsFailed.Should().BeTrue();
        handle.Errors.Should().NotBeEmpty();
        handle.Errors.Should().BeOfType<List<Error>>();
        handle.Errors[0].Code.Should().Be("Code");
        handle.Errors[0].Description.Should().Be("Description");
    }

    [Fact]
    public void HandleWithCareWithFailAndErrorList()
    {
        var handle = _handler.HandleWithCareWithFailAndErrorList();
        handle.Should().BeOfType<Result>();
        handle.IsSuccess.Should().BeFalse();
        handle.IsFailed.Should().BeTrue();
        handle.Errors.Should().NotBeEmpty();
        handle.Errors.Should().BeOfType<List<Error>>();
        handle.Errors[0].Code.Should().Be("Code");
        handle.Errors[0].Description.Should().Be("Description");
    }
}

public record CreateUserResponse(string Name);

public record CreateUserCommand(string Name);

public interface IBaseHandler<TCommand, TResponse>
{
    Result<TResponse> Handle(TCommand command);
    Result<TResponse> Success(TResponse response);
    Result<TResponse> Failure(TResponse response);
}

public abstract class BaseHandler<TCommand, TResponse> : IBaseHandler<TCommand, TResponse>
{
    public abstract Result<TResponse> Handle(TCommand command);

    public Result<TResponse> Success(TResponse response)
    {
        return Result.Ok(response);
    }

    public Result<TResponse> Failure(TResponse response)
    {
        return Result.Fail(response);
    }

    protected Result<TResponse> Failure(Error response)
    {
        return Result.Fail<TResponse>(response);
    }

    protected Result<TResponse> Failure()
    {
        return Result.Fail<TResponse>();
    }
}

public class CreateUserHandler : BaseHandler<CreateUserCommand, CreateUserResponse>
{
    public override Result<CreateUserResponse> Handle(CreateUserCommand command)
    {
        if (string.IsNullOrEmpty(command.Name))
            return Failure(FollowerErrors.RequiredName);
        return Success(new CreateUserResponse(command.Name));
    }
}

public class CreateUserHandlerUnitTests
{
    private CreateUserHandler _handler;

    public CreateUserHandlerUnitTests()
    {
        _handler = new CreateUserHandler();
    }

    [Fact]
    public void CreateUserHandle()
    {
        var command = new CreateUserCommand("Nome");
        var handle = _handler.Handle(command);
        handle.Should().BeOfType<Result<CreateUserResponse>>();
        handle.IsSuccess.Should().BeTrue();
        handle.IsFailed.Should().BeFalse();
        handle.Errors.Should().BeEmpty();
        handle.Value.Should().BeOfType<CreateUserResponse>();
        handle.Value.Name.Should().Be("Nome");
    }

    [Fact]
    public void CreateUserHandle_WithFailure()
    {
        var command = new CreateUserCommand(string.Empty);
        var handle = _handler.Handle(command);
        handle.Should().BeOfType<Result<CreateUserResponse>>();
        handle.IsSuccess.Should().BeFalse();
        handle.IsFailed.Should().BeTrue();
        handle.Errors.Should().NotBeEmpty();
        handle.Errors.Should().BeOfType<List<Error>>();
        handle.Errors[0].Code.Should().Be("Name");
        handle.Errors[0].Description.Should().Be("Name is required");
    }
}

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class StringConcatBenchmarks
{
    private const string FirstName = "John";
    private const string LastName = "Doe";

    [Benchmark]
    public string StringConcat()
    {
        return FirstName + " " + LastName;
    }

    [Benchmark]
    public string StringFormat()
    {
        return string.Format("{0} {1}", FirstName, LastName);
    }

    [Benchmark]
    public string StringInterpolation()
    {
        return $"{FirstName} {LastName}";
    }

    [Benchmark]
    public string StringConcatWithJoin()
    {
        return string.Join(" ", FirstName, LastName);
    }
}

public class BenchmarkFixture
{
    public Summary BenchmarkSummary { get; }

    public BenchmarkFixture()
    {
        var config = new ManualConfig
        {
            SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(100),
            Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest),
            Options = ConfigOptions.Default
        };
        BenchmarkSummary = BenchmarkRunner.Run<StringConcatBenchmarks>(config);
    }
}

public class StringConcatBenchmarkLiveTest : IClassFixture<BenchmarkFixture>
{
    private readonly ImmutableArray<BenchmarkReport> _benchmarkReports;
    private readonly BenchmarkReport _stringInterpolationReport;

    public StringConcatBenchmarkLiveTest(BenchmarkFixture benchmarkFixture)
    {
        _benchmarkReports = benchmarkFixture.BenchmarkSummary.Reports;
        _stringInterpolationReport = _benchmarkReports.First(x =>
            x.BenchmarkCase.Descriptor.DisplayInfo == "StringConcatBenchmarks.StringInterpolation");
    }

    [Fact]
    public void WhenBenchmarkTestsAreRun_ThenFourCasesMustBeExecuted()
    {
        var benchmarkCases = _benchmarkReports.Length;
        // Assert
        benchmarkCases.Should().Be(4);
    }

    [Fact]
    public void WhenStringInterpolationCaseIsExecuted_ThenItShouldNotTakeMoreThanFifteenNanoSecs()
    {
        var stats = _stringInterpolationReport.ResultStatistics;
        // Assert
        (stats is { Mean: < 15 }).Should().BeTrue($"Mean was {stats.Mean}");
    }

    [Fact]
    public void WhenStringInterpolationCaseIsExecuted_ThenItShouldNotConsumeMemoryMoreThanMaxAllocation()
    {
        const int maxAllocation = 1342178216;
        var memoryStats = _stringInterpolationReport.GcStats;
        var stringInterpolationCase = _stringInterpolationReport.BenchmarkCase;
        var allocation = memoryStats.GetBytesAllocatedPerOperation(stringInterpolationCase);
        var totalAllocatedBytes = memoryStats.GetTotalAllocatedBytes(true);
        // Assert
        (allocation <= maxAllocation).Should().BeTrue($"Allocation was {allocation}");
        (totalAllocatedBytes <= maxAllocation).Should().BeTrue($"TotalAllocatedBytes was {totalAllocatedBytes}");
    }

    [Fact]
    public void WhenStringInterpolationCaseIsExecuted_ThenZeroAllocationInGen1AndGen2()
    {
        var memoryStats = _stringInterpolationReport.GcStats;
        // Assert
        (memoryStats.Gen1Collections == 0).Should().BeTrue($"Gen1Collections was {memoryStats.Gen1Collections}");
        (memoryStats.Gen2Collections == 0).Should().BeTrue($"Gen2Collections was {memoryStats.Gen2Collections}");
    }
}