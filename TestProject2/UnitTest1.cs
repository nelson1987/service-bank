// namespace TestProject2;

// public class UnitTest1
// {
//     [Fact]
//     public void Test1()
//     {
//     }
// }

// public record Command(int Value);

// public record Response();

// public interface IService
// {
//     Task Send();
// }

// public abstract class HandlerBase<TResponse>
// {
//     protected Result<TResponse> Success(TResponse response)
//     {
//         return Result<TResponse>.Ok(response);
//     }

//     protected Result<TResponse> Failed(string message)
//     {
//         return Result<TResponse>.Fail(message);
//     }

//     protected Result<TResponse> Failed(Exception message)
//     {
//         return Result<TResponse>.Fail(message);
//     }
// }

// public interface IHandlerBase<TResponse, TCommand>
// {
//     Task<Result<TResponse>> Handle(TCommand command);
// }

// public class Handler(IService service) : HandlerBase<Response>, IHandlerBase<Response, Command>
// {
//     public async Task<Result<Response>> Handle(Command command)
//     {
//         try
//         {
//             if (command.Value == 0)
//                 return Failed("Value cannot be zero");
//             await service.Send();
//             return Success(new Response());
//         }
//         catch (Exception ex)
//         {
//             return Failed(ex);
//         }
//     }
// }

// public class Result<T>
// {
//     public static Result<T> Ok(T message)
//     {
//         return new Result<T>();
//     }

//     public static Result<T> Fail(string error)
//     {
//         return new Result<T>();
//     }

//     public static Result<T> Fail(Exception error)
//     {
//         return new Result<T>();
//     }
// }