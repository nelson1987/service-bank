namespace Junta.Domain;

public class Result
{
    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrWhiteSpace(error))
            throw new InvalidOperationException("Resultado de sucesso não pode conter erro");

        if (!isSuccess && string.IsNullOrWhiteSpace(error))
            throw new InvalidOperationException("Resultado de falha deve conter um erro");

        IsSuccess = isSuccess;
        Error = error;
    }

    private bool IsSuccess { get; }
    public string Error { get; }
    public bool IsFailure => !IsSuccess;

    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    public static Result Failure(string error)
    {
        return new Result(false, error);
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    public static Result<T> Failure<T>(string error)
    {
        return new Result<T>(default, false, error);
    }
}

public class Result<T> : Result
{
    protected internal Result(T value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T Value { get; }
}