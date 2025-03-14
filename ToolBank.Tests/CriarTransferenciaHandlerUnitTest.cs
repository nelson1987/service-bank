using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;

namespace ToolBank.Tests;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

public static class FollowerErrors
{
    public static readonly Error SameUser = new Error(
        "Followers.SameUser", "Can't follow yourself");

    public static readonly Error NonPublicProfile = new Error(
        "Followers.NonPublicProfile", "Can't follow non-public profiles");

    public static readonly Error AlreadyFollowing = new Error(
        "Followers.AlreadyFollowing", "Already following");

    public static Error NotFound(Guid id) => new Error(
        "ContaDebitante.NotFound", $"The follower with Id '{id}' was not found");
}

public class Result<T>
{
    private Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException(Constantes.InternalError, nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    private Result(bool isSuccess, T value)
    {
        IsSuccess = isSuccess;
        Error = Error.None;
        Value = value;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public T Value { get; }

    public static Result<T> Ok<T>() => new(true, Error.None);
    public static Result<T> Ok() => new(true, Error.None);
    public static Result<T> Ok(T value) => new(true, value);
    public static Result<T> Fail(Error error) => new(false, error);
}

public static class Constantes
{
    public const string ContaDebitanteNaoEncontrada = "Conta de origem não encontrada.";
    public const string ContaDebitanteSaldoInsuficiente = "Saldo insuficiente.";
    public const string ContaCreditanteNaoEncontrada = "Conta de destino não encontrada.";
    public const string InternalServerError = "Erro não identificado.";
    public const string InternalError = "Invalid error";
}

public record CriarTransferenciaCommand(
    Guid ContaDebitanteId,
    Guid ContaCreditanteId,
    decimal Valor);

public record CriarTransferenciaResponse;

public class Transferencia
{
    public Transferencia(
        Guid id,
        Guid contaDebitanteId,
        Guid contaCreditanteId,
        decimal valor
    )
    {
        Id = id != Guid.Empty
            ? id
            : throw new ArgumentNullException(nameof(id));
        ContaDebitanteId = contaDebitanteId != Guid.Empty
            ? contaDebitanteId
            : throw new ArgumentNullException(nameof(contaDebitanteId));
        ContaCreditanteId = contaCreditanteId != Guid.Empty
            ? contaCreditanteId
            : throw new ArgumentNullException(nameof(contaCreditanteId));
        Valor = valor >= 0
            ? valor
            : throw new ArgumentNullException(nameof(valor));
    }

    public Guid Id { get; set; }
    public Guid ContaDebitanteId { get; set; } // DebitAccountId;
    public Guid ContaCreditanteId { get; set; } // CreditAccountId;
    public decimal Valor { get; set; } // Amount;
}

public class ContaCorrente
{
    public Guid Id { get; set; }
    public decimal Saldo { get; set; }

    public bool HasSufficientBalance(decimal valor)
    {
        return valor > 0 && Saldo >= valor;
    }

    public void Withdraw(decimal valor)
    {
        Saldo -= valor;
    }

    public void Deposit(decimal valor)
    {
        Saldo += valor;
    }
}

public interface IAccountRepository
{
    ContaCorrente? GetByIdAsync(Guid accountId);
}

public interface ITransactionRepository
{
    void InsertAsync(Transferencia transaction);
}

public class CriarTransferenciaValidator : AbstractValidator<CriarTransferenciaCommand>
{
    public CriarTransferenciaValidator()
    {
        RuleFor(x => x.ContaDebitanteId)
            .NotEmpty()
            .NotNull();
        RuleFor(x => x.ContaCreditanteId)
            .NotEmpty()
            .NotNull();
        RuleFor(x => x.Valor)
            .GreaterThan(0);
    }
}

public class CriarTransferenciaValidationUnitTests
{
    private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization { ConfigureMembers = true });

    private readonly CriarTransferenciaValidator _sut;
    private readonly CriarTransferenciaCommand _command;

    public CriarTransferenciaValidationUnitTests()
    {
        _command = _fixture.Create<CriarTransferenciaCommand>();
        _sut = _fixture.Create<CriarTransferenciaValidator>();
    }

    [Fact]
    public void Given_CriarTransferenciaIsValid_When_Validation_Then_ResponseSuccess()
        => _sut
            .TestValidate(_command)
            .ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void Given_CriarTransferenciaIsValid_When_ValorIsGreaterThanZero_Then_ResponseSuccess()
        => _sut
            .TestValidate(_command with { Valor = 0.01M })
            .ShouldNotHaveValidationErrorFor(x => x.Valor);

    [Fact]
    public void Given_CriarTransferenciaIsValid_When_ContaCreditanteIdNotEmpty_Then_ResponseSuccess()
        => _sut
            .TestValidate(_command with { ContaCreditanteId = Guid.NewGuid() })
            .ShouldNotHaveValidationErrorFor(x => x.ContaCreditanteId);

    [Fact]
    public void Given_CriarTransferenciaIsValid_When_ContaDebitanteIdNotEmpty_Then_ResponseSuccess()
        => _sut
            .TestValidate(_command with { ContaDebitanteId = Guid.NewGuid() })
            .ShouldNotHaveValidationErrorFor(x => x.ContaDebitanteId);

    [Fact]
    public void Given_CriarTransferenciaIsInvalid_When_ContaCreditanteIdIsEmpty_Then_ResponseError()
        => _sut
            .TestValidate(_command with { ContaCreditanteId = Guid.Empty })
            .ShouldHaveValidationErrorFor(x => x.ContaCreditanteId)
            .Only();

    [Fact]
    public void Given_CriarTransferenciaIsInvalid_When_ContaDebitanteIdIsEmpty_Then_ResponseError()
        => _sut
            .TestValidate(_command with { ContaDebitanteId = Guid.Empty })
            .ShouldHaveValidationErrorFor(x => x.ContaDebitanteId)
            .Only();

    [Fact]
    public void Given_CriarTransferenciaIsInvalid_When_ValorIsZero_Then_ResponseError()
        => _sut
            .TestValidate(_command with { Valor = 0 })
            .ShouldHaveValidationErrorFor(x => x.Valor)
            .Only();

    [Fact]
    public void Given_CriarTransferenciaIsInvalid_When_ValorIsMinorThanZero_Then_ResponseError()
        => _sut
            .TestValidate(_command with { Valor = -0.01M })
            .ShouldHaveValidationErrorFor(x => x.Valor)
            .Only();
}

public class CriarTransferenciaHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    IValidator<CriarTransferenciaCommand> validator)
{
    public Result<CriarTransferenciaResponse> Handle(CriarTransferenciaCommand command)
    {
        try
        {
            ValidationResult validationResult = validator.Validate(command);
            if (!validationResult.IsValid)
                return Result<CriarTransferenciaResponse>.Fail(
                    new Error("", validationResult.Errors[0].ErrorMessage));

            ContaCorrente? contaDebitante = accountRepository.GetByIdAsync(command.ContaDebitanteId);
            if (contaDebitante == null)
                return Result<CriarTransferenciaResponse>.Fail(
                    FollowerErrors.NotFound(contaDebitante.Id));
            if (!contaDebitante.HasSufficientBalance(command.Valor))
                return Result<CriarTransferenciaResponse>.Fail(
                    new Error("", Constantes.ContaDebitanteSaldoInsuficiente));

            ContaCorrente? contaCreditante = accountRepository.GetByIdAsync(command.ContaCreditanteId);
            if (contaCreditante == null)
                return Result<CriarTransferenciaResponse>.Fail(new Error("", Constantes.ContaCreditanteNaoEncontrada));

            contaDebitante.Withdraw(command.Valor);
            contaCreditante.Deposit(command.Valor);

            Transferencia transactionRecord = new Transferencia(
                Guid.NewGuid(),
                contaDebitante.Id,
                contaCreditante.Id,
                command.Valor);
            transactionRepository.InsertAsync(transactionRecord);

            return Result<CriarTransferenciaResponse>.Ok(new CriarTransferenciaResponse());
        }
        catch (Exception ex)
        {
            return Result<CriarTransferenciaResponse>.Fail(new Error("", Constantes.InternalServerError));
        }
    }
}

public class CriarTransferenciaHandlerUnitTest
{
    private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization { ConfigureMembers = true });

    private readonly CriarTransferenciaCommand _command;
    private readonly CriarTransferenciaHandler _sut;
    private readonly ContaCorrente _entity;
    private readonly Mock<IAccountRepository> _accountRepository;
    private readonly Mock<ITransactionRepository> _transactionRepository;
    private readonly Mock<IValidator<CriarTransferenciaCommand>> _validator;

    public CriarTransferenciaHandlerUnitTest()
    {
        _accountRepository = _fixture.Freeze<Mock<IAccountRepository>>();
        _transactionRepository = _fixture.Freeze<Mock<ITransactionRepository>>();
        _validator = _fixture.Freeze<Mock<IValidator<CriarTransferenciaCommand>>>();
        _command = _fixture.Build<CriarTransferenciaCommand>()
            .With(x => x.Valor, 0.01M)
            .Create();
        _entity = _fixture.Build<ContaCorrente>()
            .With(x => x.Id, _command.ContaDebitanteId)
            .With(x => x.Saldo, 1.99M)
            .Create();
        _accountRepository
            .Setup(x => x.GetByIdAsync(_command.ContaDebitanteId))
            .Returns(_entity);
        _validator
            .Setup(x => x.Validate(_command))
            .Returns(new ValidationResult());
        _sut = _fixture.Create<CriarTransferenciaHandler>();
    }

    [Fact]
    public void Given_CriarTransferenciaIsValid_When_HandleEnded_Then_CriarTransferenciaResponseSuccess()
    {
        // Act
        var handler = _sut.Handle(_command);
        // Assert
        handler.Should().NotBeNull();
        handler.Should().BeOfType<Result<CriarTransferenciaResponse>>();
        handler.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Given_CriarTransferenciaIsInvalid_When_HandleEnded_Then_CriarTransferenciaResponseUnsuccess()
    {
        // Arrange
        _validator
            .Setup(v => v.Validate(_command))
            .Returns(new ValidationResult(new List<ValidationFailure>()
            {
                new("any-prop", "any-error-message")
            }));
        // Act
        var handler = _sut.Handle(_command);
        // Assert
        handler.Should().NotBeNull();
        handler.Should().BeOfType<Result<CriarTransferenciaResponse>>();
        handler.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Given_CriarTransferencia_When_DatabaseDebitante_Then_CriarTransferenciaResponseUnsuccess()
    {
        // Arrange
        _accountRepository
            .Setup(y => y.GetByIdAsync(It.Is<Guid>(x => x == _command.ContaDebitanteId)))
            .Throws(new NotImplementedException());
        // Act
        var handler = _sut.Handle(_command);
        // Assert
        handler.Should().NotBeNull();
        handler.Should().BeOfType<Result<CriarTransferenciaResponse>>();
        handler.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Given_CriarTransferencia_When_DatabaseDebitanteNotFound_Then_CriarTransferenciaResponseUnsuccess()
    {
        // Arrange
        _accountRepository
            .Setup(x => x.GetByIdAsync(It.Is<Guid>(x => x == _command.ContaDebitanteId)))
            .Returns((ContaCorrente)null!);
        // Act
        var handler = _sut.Handle(_command);
        // Assert
        handler.Should().NotBeNull();
        handler.Should().BeOfType<Result<CriarTransferenciaResponse>>();
        handler.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void
        Given_CriarTransferencia_When_DatabaseDebitanteNotHasSufficientBalance_Then_CriarTransferenciaResponseUnsuccess()
    {
        // Arrange
        _entity.Saldo = 0M;
        _accountRepository
            .Setup(x => x.GetByIdAsync(It.Is<Guid>(x => x == _command.ContaDebitanteId)))
            .Returns(_entity);
        // Act
        var handler = _sut.Handle(_command);
        // Assert
        handler.Should().NotBeNull();
        handler.Should().BeOfType<Result<CriarTransferenciaResponse>>();
        handler.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void
        Given_CriarTransferencia_When_DatabaseTransaction_Then_CriarTransferenciaResponseUnsuccess()
    {
        // Act
        var handler = _sut.Handle(_command);
        // Assert
        handler.Should().NotBeNull();
        _transactionRepository.Verify(x => x.InsertAsync(
                It.Is<Transferencia>(y => y.Valor == _command.Valor)),
            Times.Once);
    }
}