/* using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;

namespace TestProject1;

public static class Constants
{
    public const string InvalidAccountNumber = "Invalid account number";
    public const string NotEmptyFirstName = "First name cannot be empty";
}

public class CreateCashinCommand
{
    public CreateCashinCommand(string accountNumber)
    {
        AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
    }

    public string AccountNumber { get; }
}

public class CreateCashinValidation : AbstractValidator<CreateCashinCommand>
{
    public CreateCashinValidation()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .WithMessage(Constants.NotEmptyFirstName);
    }
}

public class CreateCashinResponse
{
    public CreateCashinResponse(Guid id, string accountNumber)
    {
        Id = id;
        AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
    }

    public Guid Id { get; }
    public string AccountNumber { get; }
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

public abstract class BaseHandler<TResponse>
{
    protected async Task<HandlerResponse<TResponse>> SuccessAsync(TResponse result)
    {
        return await Task.FromResult(new HandlerResponse<TResponse>(result));
    }

    protected async Task<HandlerResponse<TResponse>> FailureAsync()
    {
        return await Task.FromResult(new HandlerResponse<TResponse>(new Exception(Constants.InvalidAccountNumber)));
    }

    // protected async Task<HandlerResponse<TResponse>> SuccessIf(Boolean condition, TResponse response)
    // {
    //     return condition ? await Success(response) : await Failure();
    // }
}

public class CreateCashinHandler : BaseHandler<CreateCashinResponse>
{
    public async Task<HandlerResponse<CreateCashinResponse>> HandleAsync(CreateCashinCommand command)
    {
        var validator = await Validate(command);
        if (!validator.IsValid)
            return await FailureAsync();

        var getInfo = await GetAccountInfo();
        var createdCashin = await CreateCashin(command, getInfo);

        PersistsCreatedCashin(createdCashin);
        ProduceCreatedCashin(createdCashin);
        return await SuccessAsync(command.ToResponse(createdCashin));
    }

    private static void ProduceCreatedCashin(CashinEntity createdCashin)
    {
        var producer = new CashinProducer();
        producer.SendAsync(createdCashin.ToEvent());
    }

    private static void PersistsCreatedCashin(CashinEntity createdCashin)
    {
        var persistence = new CashinPersistence();
        persistence.SetAsync(createdCashin);
    }

    private static async Task<CashinEntity> CreateCashin(CreateCashinCommand command, GetCashinModel getInfo)
    {
        var repository = new CashinRepository();
        var createdCashin = await repository.CreateAsync(command.ToEntity(getInfo.AccountName));
        return createdCashin;
    }

    private static async Task<GetCashinModel> GetAccountInfo()
    {
        var client = new CashinHttpClient();
        var getInfo = await client.GetAsync("/api/cashin");
        return getInfo;
    }

    private static async Task<ValidationResult> Validate(CreateCashinCommand command)
    {
        var validation = new CreateCashinValidation();
        var validator = await validation.ValidateAsync(command);
        return validator;
    }
}

public static class Mapper
{
    public static CreateCashinResponse ToResponse(this CreateCashinCommand command, CashinEntity entity)
    {
        return new CreateCashinResponse(entity.Id, command.AccountNumber);
    }

    public static CashinEntity ToEntity(this CreateCashinCommand command, string accountName)
    {
        return string.IsNullOrEmpty(accountName)
            ? new CashinEntity(command.AccountNumber)
            : new CashinEntity(command.AccountNumber, accountName);
    }

    public static CreatedCashinEvent ToEvent(this CashinEntity entity)
    {
        return new CreatedCashinEvent(entity.Id, entity.AccountNumber, entity.CustomerName);
    }
}

public class CashinEntity
{
    public CashinEntity(string accountNumber)
    {
        Id = Guid.NewGuid();
        AccountNumber = accountNumber;
    }

    public CashinEntity(string accountNumber, string customerName) : this(accountNumber)
    {
        CustomerName = customerName;
    }

    public Guid Id { get; }
    public string AccountNumber { get; set; }
    public string CustomerName { get; set; }
}

public class CreatedCashinEvent
{
    public CreatedCashinEvent(Guid id, string accountNumber, string customerName)
    {
        Id = id;
        AccountNumber = accountNumber;
        CustomerName = customerName;
    }

    public Guid Id { get; }
    public string AccountNumber { get; set; }
    public string CustomerName { get; set; }
}

public class GetCashinModel
{
    public string AccountName { get; set; }
}

public class CashinRepository
{
    public async Task<CashinEntity> CreateAsync(CashinEntity entity)
    {
        return await Task.FromResult(entity);
    }
}

public class CashinPersistence
{
    public async Task<CashinEntity> SetAsync(CashinEntity entity)
    {
        return await Task.FromResult(entity);
    }
}

public class CashinProducer
{
    public async Task<CreatedCashinEvent> SendAsync(CreatedCashinEvent @event)
    {
        return await Task.FromResult(@event);
    }
}

public class CashinHttpClient
{
    public async Task<GetCashinModel> GetAsync(string path)
    {
        return await Task.FromResult(new GetCashinModel());
    }
}

#region tests

public class CreateCashinCommandUnitTest
{
    [Fact]
    public void Test1()
    {
        var req = new CreateCashinCommand("John");
        req.AccountNumber.Should().Be("John");
    }
}

public class CreateCashinValidationUnitTest
{
    private readonly CreateCashinValidation _sut = new();

    [Fact]
    public async Task WithSuccess()
    {
        var req = new CreateCashinCommand("John");
        var result = await _sut.ValidateAsync(req);
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task WithError()
    {
        var req = new CreateCashinCommand("");
        var result = await _sut.ValidateAsync(req);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}

public class CreateCashinHandlerUnitTest
{
    private readonly CreateCashinHandler _sut = new();

    [Fact]
    public async Task WithSuccess()
    {
        var req = new CreateCashinCommand("John");
        HandlerResponse<CreateCashinResponse> result = await _sut.HandleAsync(req);
        result.Value!.AccountNumber.Should().Be(req.AccountNumber);
    }

    [Fact]
    public async Task WithError()
    {
        var req = new CreateCashinCommand("");
        HandlerResponse<CreateCashinResponse> result = await _sut.HandleAsync(req);
        result.Error!.Message.Should().Be(Constants.InvalidAccountNumber);
    }
}

public class CreateCashinResponseUnitTest
{
    private readonly CashinRepository _sut = new();

    [Fact]
    public async Task WithSuccess()
    {
        var req = new CreateCashinCommand("John");
        var result = await _sut.CreateAsync(req.ToEntity("accountName"));
        result.AccountNumber.Should().Be(req.AccountNumber);
    }
}


 * Ao criar um cashin devemos receber dados de uma api com as infos da conta.
 * Inserir na base dados
 * Persistir no cache
 * Enviar evento de criacao
 *

// public interface IProducer
// {
//     string FileExists(string path);
// }
//
// public class Handler
// {
//     private readonly IProducer _producer;
//
//     public Handler(IProducer producer)
//     {
//         _producer = producer;
//     }
//
//     public string DownloadExists(string path)
//     {
//         try
//         {
//             return _producer.FileExists(path);
//         }
//         catch (Exception ex)
//         {
//             throw ex;
//         }
//     }
// }
//
// public class MockUnitTests
// {
//     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
//     private readonly Handler _sut;
//     private readonly Mock<IProducer> _producer;
//
//     public MockUnitTests()
//     {
//         _producer = _fixture.Freeze<Mock<IProducer>>();
//         _sut = _fixture.Create<Handler>();
//     }
//
//     [Fact]
//     public void WithSuccess()
//     {
//         _producer
//             .Setup(library => library.FileExists(It.IsAny<string>()))
//             .Returns(@"c:\temp\test.txt");
//         string valor = _sut.DownloadExists(@"c:\temp\test.txt2");
//         valor.Should().Be(@"c:\temp\test.txt");
//         _producer.Verify(library => library.FileExists(@"c:\temp\test.txt2"), Times.Once);
//     }
//
//     [Fact]
//     public void WithErrors()
//     {
//         _producer
//             .Setup(library => library.FileExists("ping"))
//             .Throws(new Exception("When doing operation X, the service should be pinged always"));
//         var valor = () => _sut.DownloadExists(@"ping");
//         valor.Should()
//             .ThrowExactly<Exception>()
//             .WithMessage("When doing operation X, the service should be pinged always");
//         _producer.Verify(library => library.FileExists(It.IsAny<string>()), Times.Once);
//     }
// }
//

#endregion
*/