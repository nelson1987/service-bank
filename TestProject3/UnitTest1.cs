using FluentAssertions;
using FluentResults;
using FluentValidation;
using MediatR;
using TestProject2.UseCases;

namespace TestProject3;

public static class Constantes
{
    public const string SaldoInsuficiente = "Saldo insuficiente.";
    public const string ContaOrigemInvalida = "Conta de origem inválida.";
    public const string ContaDestinoInvalida = "Conta de destino inválida.";
}

public record CreateTransferCommand(
    Guid ContaDebitanteId,
    Guid ContaCreditanteId,
    decimal Valor)
    : MediatR.IRequest<Result<CreateTransferResponse>>;

public record CreateTransferResponse(
    Guid IdTransferencia,
    DateTime DataTransferencia,
    string ContaDebitante,
    string ContaCreditante,
    decimal Valor);

public class ContaCorrente
{
    public Guid Id { get; set; }
    public decimal Saldo { get; set; } // Balance;

    /// <summary>
    /// Verifica se conta tem saldo suficiente
    /// </summary>
    /// <param name="amount"></param>
    /// <exception cref="NotImplementedException"></exception>
    public bool HasSufficientBalance(decimal amount)
    {
        return amount > 0 && Saldo >= amount;
    }

    /// <summary>
    /// Sacar
    /// </summary>
    /// <param name="amount"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Withdraw(decimal amount)
    {
        if (!HasSufficientBalance(amount))
        {
            throw new Exception("Saldo insuficiente");
        }

        Saldo -= amount;
    }

    /// <summary>
    /// Depositar
    /// </summary>
    /// <param name="amount"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Deposit(decimal amount)
    {
        Saldo += amount;
    }
}

public class Transferencia
{
    public Transferencia(
        ContaCorrente contaDebitante,
        ContaCorrente contaCreditante,
        decimal valor,
        Guid id
    )
    {
        ContaDebitante = contaDebitante ?? throw new ArgumentNullException(nameof(contaDebitante));
        contaDebitante.HasSufficientBalance(valor);
        ContaCreditante = contaCreditante ?? throw new ArgumentNullException(nameof(contaCreditante));
        Valor = valor <= 0 ? valor : throw new ArgumentNullException(nameof(valor));
        Id = id == Guid.Empty ? id : throw new ArgumentNullException(nameof(id));
    }

    public Guid Id { get; set; }
    public ContaCorrente ContaDebitante { get; set; } // DebitAccountId;
    public ContaCorrente ContaCreditante { get; set; } // CreditAccountId;
    public decimal Valor { get; set; } // Amount;
}

public class ContaCorrenteUnitTests
{
    [Fact]
    public void HasSufficientBalance()
    {
        ContaCorrente conta = new ContaCorrente();
        conta.Saldo = 0.02M;
        conta.HasSufficientBalance(0.01M).Should().Be(true);
        conta.HasSufficientBalance(0.02M).Should().Be(true);
        conta.HasSufficientBalance(0.03M).Should().Be(false);
    }

    [Fact]
    public void Withdraw_ComFundos()
    {
        ContaCorrente conta = new ContaCorrente();
        conta.Saldo = 0.02M;
        conta.Withdraw(0.01M);
        conta.Saldo.Should().Be(0.01M);
    }

    [Fact]
    public void Withdraw_SemFundos_RaisedException()
    {
        ContaCorrente conta = new ContaCorrente();
        conta.Saldo = 0.02M;
        conta.Withdraw(0.03M);
        //conta.Saldo.Should().Be(0.03M);
    }

    [Fact]
    public void Deposit_IndependeDeFundos()
    {
        ContaCorrente conta = new ContaCorrente();
        conta.Saldo = 0.00M;
        conta.Deposit(0.01M);
        conta.Saldo.Should().Be(0.01M);
        conta.Deposit(0.01M);
        conta.Saldo.Should().Be(0.02M);
    }
}

public class TransferenciaUnitTests
{
    [Fact]
    public void Create()
    {
        ContaCorrente debitante = new ContaCorrente();
        ContaCorrente creditante = new ContaCorrente();
        Transferencia transferencia = new Transferencia(debitante, creditante, 123.45M, Guid.NewGuid());
    }
}

public record CreatedUsuarioEvent(string Nome);

public interface IAccountRepository
{
    Task<ContaCorrente?> GetByIdAsync(Guid accountId);
}

public interface ITransactionRepository
{
    Task InsertAsync(Transferencia transaction);
}

public interface IPublisher
{
    Task<Result> SendAsync(CreatedUsuarioEvent message, CancellationToken cancellationToken = default);
}

public interface IPersistence
{
    Task<Result> SetAsync(CreateTransferResponse response, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task CommitAsync();
    Task BeginTransactionAsync();
}

public class CreateUsuarioCommandValidation : AbstractValidator<CreateTransferCommand>
{
    public CreateUsuarioCommandValidation()
    {
        RuleFor(x => x.ContaDebitante)
            .NotEmpty()
            .NotNull();
        RuleFor(x => x.ContaCreditante)
            .NotEmpty()
            .NotNull();
        RuleFor(x => x.Valor)
            .GreaterThan(0);
    }
}

public interface ITransaction : IAsyncDisposable
{
    Task BeginTransactionAsync();
    Task CommitTransaction();
}

public class CreateUsuarioHandler : IRequestHandler<CreateTransferCommand, Result<CreateUsuarioResponse>>
{
    private readonly IAccountRepository _accountRepo;
    private readonly IPublisher _publisher;
    private readonly IPersistence _persistence;
    private readonly IValidator<CreateTransferCommand> _validator;
    private readonly ITransactionRepository _transactionRepo;
    private readonly ITransaction _transaction;

    public async Task<Result<CreateUsuarioResponse>> Handle(CreateTransferCommand request,
        CancellationToken cancellationToken = default)
    {
        using var transacao = await _transaction.BeginTransactionAsync();
        try
        {
            // Validar Command
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

            var sourceAccount = await _accountRepo.GetByIdAsync(request.ContaDebitanteId);
            if (sourceAccount == null)
                return Result.Fail(Constantes.ContaOrigemInvalida);
            if (!sourceAccount.HasSufficientBalance(request.Valor))
                return Result.Fail(Constantes.SaldoInsuficiente);
            var destinationAccount = await _accountRepo.GetByIdAsync(request.ContaCreditanteId);
            if (destinationAccount == null)
                return Result.Fail(Constantes.ContaDestinoInvalida);

            // Atualiza saldos
            sourceAccount.Withdraw(request.Valor);
            destinationAccount.Deposit(request.Valor);

            var transactionRecord = new Transferencia(
                sourceAccount,
                destinationAccount,
                request.Valor,
                Guid.NewGuid());
            await _transactionRepo.InsertAsync(transactionRecord);

            await _transaction.CommitTransaction();
            /*
            // Inserir Entidade na Base
            var dataBaseResult = await _repository.CreateAsync(new ContaCorrente(), cancellationToken);
            if (dataBaseResult.IsSuccess)
                return Result.Ok(new CreateUsuarioResponse());

            // Enviar Evento
            var eventResult =
                await _publisher.SendAsync(new CreatedUsuarioEvent(request.ContaDebitante), cancellationToken);
            if (eventResult.IsSuccess)
                return Result.Ok(new CreateUsuarioResponse());

            // Enviar Evento
            var cacheResult = await _persistence.SetAsync(new CreateUsuarioResponse(), cancellationToken);
            if (cacheResult.IsSuccess)
                return Result.Ok(new CreateUsuarioResponse());
            */
            return Result.Ok(new CreateTransferResponse());
        }
        catch (Exception ex)
        {
            await _transaction.BeginTransactionAsync();
            return Result.Fail(ex.Message);
        }
    }
}
/*
public class UsuarioControllerUnitTests
{
    private readonly UsuarioController controller;

    public UsuarioControllerUnitTests()
    {
        controller = new UsuarioController();
    }

    [Fact]
    public async Task Post_throws_exception_if_user_is_null()
    {
        var command = new CreateUsuarioCommand();
        var cancellationToken = CancellationToken.None;
        CreateUsuarioResponse result = await controller.Post(command, cancellationToken);
        result.Should().BeOfType<CreateUsuarioResponse>();
        result.Should().NotBeNull();
    }
}
*/