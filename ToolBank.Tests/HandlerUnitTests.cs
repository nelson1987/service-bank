/*
 using FluentAssertions;

namespace ToolBank.Tests.HandlerTests;

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
    public bool IsFailed => Reasons.OfType<Error>().Any();
    public bool IsSuccess => !IsFailed;
    public List<Error> Errors => Reasons.OfType<IError>().ToList();

    protected ResultBase()
    {
        Reasons = new List<Reason>();
    }

    public bool HasError<TError>() where TError : Error
    {
        return HasError<TError>(out _);
    }

    public bool HasError<TError>(out IEnumerable<TError> result) where TError : Error
    {
        return HasError<TError>(e => true, out result);
    }

    public bool HasError<TError>(Func<TError, bool> predicate) where TError : Error
    {
        return HasError<TError>(predicate, out _);
    }

    public bool HasError<TError>(Func<TError, bool> predicate, out IEnumerable<TError> result) where TError : Error
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return ResultHelper.HasError(Errors, predicate, out result);
    }

    public bool HasError(Func<Error, bool> predicate)
    {
        return HasError(predicate, out _);
    }

    public bool HasError(Func<Error, bool> predicate, out IEnumerable<Error> result)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return ResultHelper.HasError(Errors, predicate, out result);
    }

    public bool HasException<TException>() where TException : Exception
    {
        return HasException<TException>(out _);
    }

    public bool HasException<TException>(out IEnumerable<Error> result) where TException : Exception
    {
        return HasException<TException>(error => true, out result);
    }

    public bool HasException<TException>(Func<TException, bool> predicate) where TException : Exception
    {
        return HasException(predicate, out _);
    }

    public bool HasException<TException>(Func<TException, bool> predicate, out IEnumerable<IError> result)
        where TException : Exception
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return ResultHelper.HasException(Errors, predicate, out result);
    }


    /// <summary>
    /// Check if the result object contains a success from a specific type
    /// </summary>
    public bool HasSuccess<TSuccess>() where TSuccess : ISuccess
    {
        return HasSuccess<TSuccess>(success => true, out _);
    }

    /// <summary>
    /// Check if the result object contains a success from a specific type
    /// </summary>
    public bool HasSuccess<TSuccess>(out IEnumerable<TSuccess> result) where TSuccess : ISuccess
    {
        return HasSuccess<TSuccess>(success => true, out result);
    }

    /// <summary>
    /// Check if the result object contains a success from a specific type and with a specific condition
    /// </summary>
    public bool HasSuccess<TSuccess>(Func<TSuccess, bool> predicate) where TSuccess : ISuccess
    {
        return HasSuccess(predicate, out _);
    }

    /// <summary>
    /// Check if the result object contains a success from a specific type and with a specific condition
    /// </summary>
    public bool HasSuccess<TSuccess>(Func<TSuccess, bool> predicate, out IEnumerable<TSuccess> result)
        where TSuccess : ISuccess
    {
        return ResultHelper.HasSuccess(Successes, predicate, out result);
    }

    /// <summary>
    /// Check if the result object contains a success with a specific condition
    /// </summary>
    public bool HasSuccess(Func<ISuccess, bool> predicate, out IEnumerable<ISuccess> result)
    {
        return ResultHelper.HasSuccess(Successes, predicate, out result);
    }

    /// <summary>
    /// Check if the result object contains a success with a specific condition
    /// </summary>
    public bool HasSuccess(Func<ISuccess, bool> predicate)
    {
        return ResultHelper.HasSuccess(Successes, predicate, out _);
    }

    /// <summary>
    /// Deconstruct Result 
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="isFailed"></param>
    public void Deconstruct(out bool isSuccess, out bool isFailed)
    {
        isSuccess = IsSuccess;
        isFailed = IsFailed;
    }

    /// <summary>
    /// Deconstruct Result
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="isFailed"></param>
    /// <param name="errors"></param>
    public void Deconstruct(out bool isSuccess, out bool isFailed, out List<IError> errors)
    {
        isSuccess = IsSuccess;
        isFailed = IsFailed;
        errors = IsFailed ? Errors : default;
    }
}

public abstract class ResultBase<TResult> : ResultBase
    where TResult : ResultBase<TResult>

{
    /// <summary>
    /// Add a reason (success or error)
    /// </summary>
    public TResult WithReason(IReason reason)
    {
        Reasons.Add(reason);
        return (TResult)this;
    }

    /// <summary>
    /// Add multiple reasons (success or error)
    /// </summary>
    public TResult WithReasons(IEnumerable<IReason> reasons)
    {
        Reasons.AddRange(reasons);
        return (TResult)this;
    }

    /// <summary>
    /// Add an error
    /// </summary>
    public TResult WithError(string errorMessage)
    {
        return WithError(Result.Settings.ErrorFactory(errorMessage));
    }

    /// <summary>
    /// Add an error
    /// </summary>
    public TResult WithError(Error error)
    {
        return WithReason(error);
    }

    /// <summary>
    /// Add multiple errors
    /// </summary>
    public TResult WithErrors(IEnumerable<IError> errors)
    {
        return WithReasons(errors);
    }

    /// <summary>
    /// Add multiple errors
    /// </summary>
    public TResult WithErrors(IEnumerable<string> errors)
    {
        return WithReasons(errors.Select(errorMessage => Result.Settings.ErrorFactory(errorMessage)));
    }

    /// <summary>
    /// Add an error
    /// </summary>
    public TResult WithError<TError>()
        where TError : IError, new()
    {
        return WithError(new TError());
    }

    /// <summary>
    /// Add a success
    /// </summary>
    public TResult WithSuccess(string successMessage)
    {
        return WithSuccess(Result.Settings.SuccessFactory(successMessage));
    }

    /// <summary>
    /// Add a success
    /// </summary>
    public TResult WithSuccess(ISuccess success)
    {
        return WithReason(success);
    }

    /// <summary>
    /// Add a success
    /// </summary>
    public TResult WithSuccess<TSuccess>()
        where TSuccess : Success, new()
    {
        return WithSuccess(new TSuccess());
    }

    /// <summary>
    /// Add multiple successes
    /// </summary>
    public TResult WithSuccesses(IEnumerable<ISuccess> successes)
    {
        foreach (var success in successes)
        {
            WithSuccess(success);
        }

        return (TResult)this;
    }

    /// <summary>
    /// Log the result. Configure the logger via Result.Setup(..)
    /// </summary>
    public TResult Log(LogLevel logLevel = LogLevel.Information)
    {
        return Log(string.Empty, null, logLevel);
    }

    /// <summary>
    /// Log the result. Configure the logger via Result.Setup(..)
    /// </summary>
    public TResult Log(string context, LogLevel logLevel = LogLevel.Information)
    {
        return Log(context, null, logLevel);
    }

    /// <summary>
    /// Log the result with a specific logger context. Configure the logger via Result.Setup(..)
    /// </summary>
    public TResult Log(string context, string content, LogLevel logLevel = LogLevel.Information)
    {
        var logger = Result.Settings.Logger;

        logger.Log(context, content, this, logLevel);

        return (TResult)this;
    }

    /// <summary>
    /// Log the result with a typed context. Configure the logger via Result.Setup(..)
    /// </summary>
    public TResult Log<TContext>(LogLevel logLevel = LogLevel.Information)
    {
        return Log<TContext>(null, logLevel);
    }

    /// <summary>
    /// Log the result with a typed context. Configure the logger via Result.Setup(..)
    /// </summary>
    public TResult Log<TContext>(string content, LogLevel logLevel = LogLevel.Information)
    {
        var logger = Result.Settings.Logger;

        logger.Log<TContext>(content, this, logLevel);

        return (TResult)this;
    }

    /// <summary>
    /// Log the result only when it is successful. Configure the logger via Result.Setup(..)
    /// </summary>
    public TResult LogIfSuccess(LogLevel logLevel = LogLevel.Information)
    {
        if (IsSuccess)
            return Log(logLevel);

        return (TResult)this;
    }

    /// <summary>
    /// Log the result with a specific logger context only when it is successful. Configure the logger via Result.Setup(..)
    /// </summary>
    public TResult LogIfSuccess(string context, string content = null, LogLevel logLevel = LogLevel.Information)
    {
        if (IsSuccess)
            return Log(context, content, logLevel);

        return (TResult)this;
    }

    /// <summary>
    /// Log the result with a typed context only when it is successful. Configure the logger via Result.Setup(..)
    /// </summary>
    public TResult LogIfSuccess<TContext>(string content = null, LogLevel logLevel = LogLevel.Information)
    {
        if (IsSuccess)
            return Log<TContext>(content, logLevel);

        return (TResult)this;
    }

    /// <summary>
    /// Log the result only when it is failed. Configure the logger via Result.Setup(..)
    /// </summary>
    public TResult LogIfFailed(LogLevel logLevel = LogLevel.Error)
    {
        if (IsFailed)
            return Log(logLevel);

        return (TResult)this;
    }

    /// <summary>
    /// Log the result with a specific logger context only when it is failed. Configure the logger via Result.Setup(..)
    /// </summary>
    public TResult LogIfFailed(string context, string content = null, LogLevel logLevel = LogLevel.Error)
    {
        if (IsFailed)
            return Log(context, content, logLevel);

        return (TResult)this;
    }

    /// <summary>
    /// Log the result with a typed context only when it is failed. Configure the logger via Result.Setup(..)
    /// </summary>
    public TResult LogIfFailed<TContext>(string content = null, LogLevel logLevel = LogLevel.Error)
    {
        if (IsFailed)
            return Log<TContext>(content, logLevel);

        return (TResult)this;
    }

    /// <summary>
    /// ToString override
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var reasonsString = Reasons.Any()
            ? $", Reasons='{ReasonFormat.ReasonsToString(Reasons)}'"
            : string.Empty;

        return $"Result: IsSuccess='{IsSuccess}'{reasonsString}";
    }
}

public partial class Result : ResultBase<Result>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Result()
    {
    }

    /// <summary>
    /// Map all errors of the result via errorMapper
    /// </summary>
    /// <param name="errorMapper"></param>
    /// <returns></returns>
    public Result MapErrors(Func<IError, IError> errorMapper)
    {
        if (IsSuccess)
            return this;

        return new Result()
            .WithErrors(Errors.Select(errorMapper))
            .WithSuccesses(Successes);
    }

    /// <summary>
    /// Map all successes of the result via successMapper
    /// </summary>
    /// <param name="successMapper"></param>
    /// <returns></returns>
    public Result MapSuccesses(Func<ISuccess, ISuccess> successMapper)
    {
        return new Result()
            .WithErrors(Errors)
            .WithSuccesses(Successes.Select(successMapper));
    }

    /// <summary>
    /// Convert result without value to a result containing a value
    /// </summary>
    /// <typeparam name="TNewValue">Type of the value</typeparam>
    /// <param name="newValue">Value to add to the new result</param>
    public Result<TNewValue> ToResult<TNewValue>(TNewValue newValue = default)
    {
        return new Result<TNewValue>()
            .WithValue(IsFailed ? default : newValue)
            .WithReasons(Reasons);
    }

    /// <summary>
    /// Convert result to result with value that may fail.
    /// </summary>
    /// <example>
    /// <code>
    ///  var bakeryDtoResult = result.Bind(GetWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="bind">Transformation that may fail.</param>
    public Result<TNewValue> Bind<TNewValue>(Func<Result<TNewValue>> bind)
    {
        var result = new Result<TNewValue>();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = bind();
            result.WithValue(converted.ValueOrDefault);
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// Convert result to result with value that may fail asynchronously.
    /// </summary>
    /// <example>
    /// <code>
    ///  var bakeryDtoResult = result.Bind(GetWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="bind">Transformation that may fail.</param>
    public async Task<Result<TNewValue>> Bind<TNewValue>(Func<Task<Result<TNewValue>>> bind)
    {
        var result = new Result<TNewValue>();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = await bind();
            result.WithValue(converted.ValueOrDefault);
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// Convert result to result with value that may fail asynchronously.
    /// </summary>
    /// <example>
    /// <code>
    ///  var bakeryDtoResult = result.Bind(GetWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="bind">Transformation that may fail.</param>
    public async ValueTask<Result<TNewValue>> Bind<TNewValue>(Func<ValueTask<Result<TNewValue>>> bind)
    {
        var result = new Result<TNewValue>();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = await bind();
            result.WithValue(converted.ValueOrDefault);
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// Execute an action which returns a <see cref="Result"/>.
    /// </summary>
    /// <example>
    /// <code>
    ///  var done = result.Bind(ActionWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="action">Action that may fail.</param>
    public Result Bind(Func<Result> action)
    {
        var result = new Result();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = action();
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// Execute an action which returns a <see cref="Result"/> asynchronously.
    /// </summary>
    /// <example>
    /// <code>
    ///  var done = result.Bind(ActionWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="action">Action that may fail.</param>
    public async Task<Result> Bind(Func<Task<Result>> action)
    {
        var result = new Result();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = await action();
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// Execute an action which returns a <see cref="Result"/> asynchronously.
    /// </summary>
    /// <example>
    /// <code>
    ///  var done = result.Bind(ActionWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="action">Action that may fail.</param>
    public async ValueTask<Result> Bind(Func<ValueTask<Result>> action)
    {
        var result = new Result();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = await action();
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// Implict conversion from <see cref="Error"/> to a <see cref="Result"/>
    /// </summary>
    /// <param name="error">The error</param>
    public static implicit operator Result(Error error)
    {
        return Fail(error);
    }

    /// <summary>
    /// Implict conversion from <see cref="List{Error}"/> to a <see cref="Result"/>
    /// </summary>
    /// <param name="errors">The errors</param>
    public static implicit operator Result(List<Error> errors)
    {
        return Fail(errors);
    }
}

public interface IResult<out TValue> : IResultBase
{
    /// <summary>
    /// Get the Value. If result is failed then an Exception is thrown because a failed result has no value. Opposite see property ValueOrDefault.
    /// </summary>
    TValue Value { get; }

    /// <summary>
    /// Get the Value. If result is failed then a default value is returned. Opposite see property Value.
    /// </summary>
    TValue ValueOrDefault { get; }
}

public class Result<TValue> : ResultBase<Result<TValue>>, IResult<TValue>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Result()
    {
    }

    private TValue _value;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TValue ValueOrDefault => _value;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TValue Value
    {
        get
        {
            ThrowIfFailed();

            return _value;
        }
        private set
        {
            ThrowIfFailed();

            _value = value;
        }
    }

    /// <summary>
    /// Set value
    /// </summary>
    public Result<TValue> WithValue(TValue value)
    {
        Value = value;
        return this;
    }

    /// <summary>
    /// Map all errors of the result via errorMapper
    /// </summary>
    /// <param name="errorMapper"></param>
    /// <returns></returns>
    public Result<TValue> MapErrors(Func<IError, IError> errorMapper)
    {
        if (IsSuccess)
            return this;

        return new Result<TValue>()
            .WithErrors(Errors.Select(errorMapper))
            .WithSuccesses(Successes);
    }

    /// <summary>
    /// Map all successes of the result via successMapper
    /// </summary>
    /// <param name="successMapper"></param>
    /// <returns></returns>
    public Result<TValue> MapSuccesses(Func<ISuccess, ISuccess> successMapper)
    {
        return new Result<TValue>()
            .WithValue(ValueOrDefault)
            .WithErrors(Errors)
            .WithSuccesses(Successes.Select(successMapper));
    }

    /// <summary>
    /// Convert result with value to result without value
    /// </summary>
    public Result ToResult()
    {
        return new Result()
            .WithReasons(Reasons);
    }

    /// <summary>
    /// Convert result with value to result with another value. Use valueConverter parameter to specify the value transformation logic.
    /// </summary>
    public Result<TNewValue> ToResult<TNewValue>(Func<TValue, TNewValue> valueConverter = null)
    {
        return Map(valueConverter);
    }

    /// <summary>
    /// Convert result with value to result with another value. Use valueConverter parameter to specify the value transformation logic.
    /// </summary>
    public Result<TNewValue> Map<TNewValue>(Func<TValue, TNewValue> mapLogic)
    {
        if (IsSuccess && mapLogic == null)
            throw new ArgumentException("If result is success then valueConverter should not be null");

        return new Result<TNewValue>()
            .WithValue(IsFailed ? default : mapLogic(Value))
            .WithReasons(Reasons);
    }

    /// <summary>
    /// Convert result with value to result with another value that may fail.
    /// </summary>
    /// <example>
    /// <code>
    ///  var bakeryDtoResult = result
    ///     .Bind(GetWhichMayFail)
    ///     .Bind(ProcessWhichMayFail)
    ///     .Bind(FormattingWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="bind">Transformation that may fail.</param>
    public Result<TNewValue> Bind<TNewValue>(Func<TValue, Result<TNewValue>> bind)
    {
        var result = new Result<TNewValue>();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = bind(Value);
            result.WithValue(converted.ValueOrDefault);
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// Convert result with value to result with another value that may fail asynchronously.
    /// </summary>
    /// <example>
    /// <code>
    ///  var bakeryDtoResult = await result.Bind(GetWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="bind">Transformation that may fail.</param>
    public async Task<Result<TNewValue>> Bind<TNewValue>(Func<TValue, Task<Result<TNewValue>>> bind)
    {
        var result = new Result<TNewValue>();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = await bind(Value);
            result.WithValue(converted.ValueOrDefault);
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// Convert result with value to result with another value that may fail asynchronously.
    /// </summary>
    /// <example>
    /// <code>
    ///  var bakeryDtoResult = await result.Bind(GetWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="bind">Transformation that may fail.</param>
    public async ValueTask<Result<TNewValue>> Bind<TNewValue>(Func<TValue, ValueTask<Result<TNewValue>>> bind)
    {
        var result = new Result<TNewValue>();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = await bind(Value);
            result.WithValue(converted.ValueOrDefault);
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// Execute an action which returns a <see cref="Result"/>.
    /// </summary>
    /// <example>
    /// <code>
    ///  var done = result.Bind(ActionWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="action">Action that may fail.</param>
    public Result Bind(Func<TValue, Result> action)
    {
        var result = new Result();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = action(Value);
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// Execute an action which returns a <see cref="Result"/> asynchronously.
    /// </summary>
    /// <example>
    /// <code>
    ///  var done = await result.Bind(ActionWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="action">Action that may fail.</param>
    public async Task<Result> Bind(Func<TValue, Task<Result>> action)
    {
        var result = new Result();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = await action(Value);
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// Execute an action which returns a <see cref="Result"/> asynchronously.
    /// </summary>
    /// <example>
    /// <code>
    ///  var done = await result.Bind(ActionWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="action">Action that may fail.</param>
    public async ValueTask<Result> Bind(Func<TValue, ValueTask<Result>> action)
    {
        var result = new Result();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = await action(Value);
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    /// ToString implementation
    /// </summary>
    public override string ToString()
    {
        var baseString = base.ToString();
        var valueString = ValueOrDefault.ToLabelValueStringOrEmpty(nameof(Value));
        return $"{baseString}, {valueString}";
    }

    /// <summary>
    /// Implicit conversion from <see cref="Result"/> without a value to <see cref="Result{TValue}"/> having the default value
    /// </summary>
    public static implicit operator Result<TValue>(Result result)
    {
        return result.ToResult<TValue>(default);
    }

    /// <summary>
    /// Implicit conversion from <see cref="Result{TValue}"/> having a value to <see cref="Result"/> without a value
    /// </summary>
    public static implicit operator Result<object>(Result<TValue> result)
    {
        return result.ToResult<object>(value => value);
    }

    /// <summary>
    /// Implicit conversion of a value to <see cref="Result{TValue}"/>
    /// </summary>
    public static implicit operator Result<TValue>(TValue value)
    {
        if (value is Result<TValue> r)
            return r;

        return Result.Ok(value);
    }

    /// <summary>
    /// Implicit conversion of an <see cref="Error"/> to <see cref="Result{TValue}"/>
    /// </summary>
    public static implicit operator Result<TValue>(Error error)
    {
        return Result.Fail(error);
    }

    /// <summary>
    /// Implicit conversion of a list of <see cref="Error"/> to <see cref="Result{TValue}"/>
    /// </summary>
    public static implicit operator Result<TValue>(List<Error> errors)
    {
        return Result.Fail(errors);
    }

    /// <summary>
    /// Deconstruct Result
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="isFailed"></param>
    /// <param name="value"></param>
    public void Deconstruct(out bool isSuccess, out bool isFailed, out TValue value)
    {
        isSuccess = IsSuccess;
        isFailed = IsFailed;
        value = IsSuccess ? Value : default;
    }

    /// <summary>
    /// Deconstruct Result
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="isFailed"></param>
    /// <param name="value"></param>
    /// <param name="errors"></param>
    public void Deconstruct(out bool isSuccess, out bool isFailed, out TValue value, out List<IError> errors)
    {
        isSuccess = IsSuccess;
        isFailed = IsFailed;
        value = IsSuccess ? Value : default;
        errors = IsFailed ? Errors : default;
    }

    private void ThrowIfFailed()
    {
        if (IsFailed)
            throw new InvalidOperationException(
                $"Result is in status failed. Value is not set. Having: {ReasonFormat.ErrorReasonsToString(Errors)}");
    }
}

public record Response();

public partial class Result
{
    internal static ResultSettings Settings { get; private set; }

    static Result()
    {
        Settings = new ResultSettingsBuilder().Build();
    }

    /// <summary>
    /// Setup global settings like logging
    /// </summary>
    public static void Setup(Action<ResultSettingsBuilder> setupFunc)
    {
        var settingsBuilder = new ResultSettingsBuilder();
        setupFunc(settingsBuilder);

        Settings = settingsBuilder.Build();
    }

    /// <summary>
    /// Creates a success result
    /// </summary>
    public static Result Ok()
    {
        return new Result();
    }

    /// <summary>
    /// Creates a failed result with the given error
    /// </summary>
    public static Result Fail(Error error)
    {
        var result = new Result();
        result.WithError(error);
        return result;
    }

    /// <summary>
    /// Creates a failed result with the given error message. Internally an error object from the error factory is created. 
    /// </summary>
    public static Result Fail(string errorMessage)
    {
        var result = new Result();
        result.WithError(Settings.ErrorFactory(errorMessage));
        return result;
    }

    /// <summary>
    /// Creates a failed result with the given error messages. Internally a list of error objects from the error factory is created
    /// </summary>
    public static Result Fail(IEnumerable<string> errorMessages)
    {
        if (errorMessages == null)
            throw new ArgumentNullException(nameof(errorMessages), "The list of error messages cannot be null");

        var result = new Result();
        result.WithErrors(errorMessages.Select(Settings.ErrorFactory));
        return result;
    }

    /// <summary>
    /// Creates a failed result with the given errors.
    /// </summary>
    public static Result Fail(IEnumerable<IError> errors)
    {
        if (errors == null)
            throw new ArgumentNullException(nameof(errors), "The list of errors cannot be null");

        var result = new Result();
        result.WithErrors(errors);
        return result;
    }

    /// <summary>
    /// Creates a success result with the given value
    /// </summary>
    public static Result<TValue> Ok<TValue>(TValue value)
    {
        var result = new Result<TValue>();
        result.WithValue(value);
        return result;
    }

    /// <summary>
    /// Creates a failed result with the given error
    /// </summary>
    public static Result<TValue> Fail<TValue>(IError error)
    {
        var result = new Result<TValue>();
        result.WithError(error);
        return result;
    }

    /// <summary>
    /// Creates a failed result with the given error message. Internally an error object from the error factory is created. 
    /// </summary>
    public static Result<TValue> Fail<TValue>(string errorMessage)
    {
        var result = new Result<TValue>();
        result.WithError(Settings.ErrorFactory(errorMessage));
        return result;
    }

    /// <summary>
    /// Creates a failed result with the given error messages. Internally a list of error objects from the error factory is created. 
    /// </summary>
    public static Result<TValue> Fail<TValue>(IEnumerable<string> errorMessages)
    {
        if (errorMessages == null)
            throw new ArgumentNullException(nameof(errorMessages), "The list of error messages cannot be null");

        var result = new Result<TValue>();
        result.WithErrors(errorMessages.Select(Settings.ErrorFactory));
        return result;
    }

    /// <summary>
    /// Creates a failed result with the given errors.
    /// </summary>
    public static Result<TValue> Fail<TValue>(IEnumerable<IError> errors)
    {
        if (errors == null)
            throw new ArgumentNullException(nameof(errors), "The list of errors cannot be null");

        var result = new Result<TValue>();
        result.WithErrors(errors);
        return result;
    }

    /// <summary>
    /// Merge multiple result objects to one result object together
    /// </summary>
    public static Result Merge(params ResultBase[] results)
    {
        return ResultHelper.Merge(results);
    }

    /// <summary>
    /// Merge multiple result objects to one result object together. Return one result with a list of merged values.
    /// </summary>
    public static Result<IEnumerable<TValue>> Merge<TValue>(params Result<TValue>[] results)
    {
        return ResultHelper.MergeWithValue(results);
    }

    /// <summary>
    /// Create a success/failed result depending on the parameter isSuccess
    /// </summary>
    public static Result OkIf(bool isSuccess, IError error)
    {
        return isSuccess ? Ok() : Fail(error);
    }

    /// <summary>
    /// Create a success/failed result depending on the parameter isSuccess
    /// </summary>
    public static Result OkIf(bool isSuccess, string error)
    {
        return isSuccess ? Ok() : Fail(error);
    }

    /// <summary>
    /// Create a success/failed result depending on the parameter isSuccess
    /// </summary>
    /// <remarks>
    /// Error is lazily evaluated.
    /// </remarks>
    public static Result OkIf(bool isSuccess, Func<IError> errorFactory)
    {
        return isSuccess ? Ok() : Fail(errorFactory.Invoke());
    }

    /// <summary>
    /// Create a success/failed result depending on the parameter isSuccess
    /// </summary>
    /// <remarks>
    /// Error is lazily evaluated.
    /// </remarks>
    public static Result OkIf(bool isSuccess, Func<string> errorMessageFactory)
    {
        return isSuccess ? Ok() : Fail(errorMessageFactory.Invoke());
    }

    /// <summary>
    /// Create a success/failed result depending on the parameter isFailure
    /// </summary>
    public static Result FailIf(bool isFailure, IError error)
    {
        return isFailure ? Fail(error) : Ok();
    }

    /// <summary>
    /// Create a success/failed result depending on the parameter isFailure
    /// </summary>
    public static Result FailIf(bool isFailure, string error)
    {
        return isFailure ? Fail(error) : Ok();
    }

    /// <summary>
    /// Create a success/failed result depending on the parameter isFailure
    /// </summary>
    /// <remarks>
    /// Error is lazily evaluated.
    /// </remarks>
    public static Result FailIf(bool isFailure, Func<IError> errorFactory)
    {
        return isFailure ? Fail(errorFactory.Invoke()) : Ok();
    }

    /// <summary>
    /// Create a success/failed result depending on the parameter isFailure
    /// </summary>
    /// <remarks>
    /// Error is lazily evaluated.
    /// </remarks>
    public static Result FailIf(bool isFailure, Func<string> errorMessageFactory)
    {
        return isFailure ? Fail(errorMessageFactory.Invoke()) : Ok();
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static Result Try(Action action, Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            action();
            return Ok();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static async Task<Result> Try(Func<Task> action, Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            await action();
            return Ok();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static async ValueTask<Result> Try(Func<ValueTask> action, Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            await action();
            return Ok();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static Result<T> Try<T>(Func<T> action, Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return Ok(action());
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static async Task<Result<T>> Try<T>(Func<Task<T>> action, Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return Ok(await action());
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static async ValueTask<Result<T>> Try<T>(Func<ValueTask<T>> action,
        Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return Ok(await action());
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static Result Try(Func<Result> action, Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return action();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static async Task<Result> Try(Func<Task<Result>> action, Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return await action();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static async ValueTask<Result> Try(Func<ValueTask<Result>> action,
        Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return await action();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static Result<T> Try<T>(Func<Result<T>> action, Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return action();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static async Task<Result<T>> Try<T>(Func<Task<Result<T>>> action,
        Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return await action();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    /// Executes the action. If an exception is thrown within the action then this exception is transformed via the catchHandler to an Error object
    /// </summary>
    public static async ValueTask<Result<T>> Try<T>(Func<ValueTask<Result<T>>> action,
        Func<Exception, IError> catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return await action();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }
}

public class Handler
{
    public Result Handle()
    {
        return Result.Fail("");
    }

    public Result<Response> HandleWithCare()
    {
        return Result.Ok(new Response());
    }
}

public class HandlerUnitTests
{
    [Fact]
    public void Post()
    {
        var handler = new Handler();
        var result = handler.Handle();
        result.IsSuccess.Should().BeTrue();
    }
}

public class ErrorUnitTests
{
    [Fact]
    public void PostError_ReturnProperties()
    {
        Error error = new("error", "error description");
        error.Code.Should().Be("error");
        error.Description.Should().Be("error description");
    }

    [Fact]
    public void PostErrorNone_ReturnEmptyProperties()
    {
        Error error = Error.None;
        error.Code.Should().BeEmpty();
        error.Description.Should().BeEmpty();
    }
}
*/