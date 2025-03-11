namespace TestProject2.UseCases;

public record Result();

public interface IRequest<out TResponse>
{
}

public record TransferAmountCommand(
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount) : IRequest<Result>;

public class Transaction
{
    public Guid SourceAccountId;
    public Guid DestinationAccountId;
    public decimal Amount;
    public TransactionType Type;
    public TransactionStatus Status;
};

public enum TransactionType
{
    Transfer = 1,
}

public enum TransactionStatus
{
    Completed = 1,
}

public class Account
{
    public Guid Id { get; set; }

    public void HasSufficientBalance(decimal amount)
    {
        throw new NotImplementedException();
    }

    public void Withdraw(decimal amount)
    {
        throw new NotImplementedException();
    }

    public void Deposit(decimal amount)
    {
        throw new NotImplementedException();
    }
}

public interface IAccountRepository
{
    Task<Account> GetByIdAsync(Guid accountId);
}

public interface ITransactionRepository
{
    Task InsertAsync(Transaction transaction);
}

public interface IUnitOfWork
{
    Task CommitAsync();
    Task<IResolved> BeginTransactionAsync();
}

public interface IResolved : IAsyncDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}

public interface IRequestHandler<in TRequest, out TResponse>
{
}

public class TransferAmountCommandHandler
    : IRequestHandler<TransferAmountCommand, Result>
{
    private readonly IAccountRepository _accountRepo;
    private readonly ITransactionRepository _transactionRepo;
    private readonly IUnitOfWork _uow;

    public TransferAmountCommandHandler(
        IAccountRepository accountRepo,
        ITransactionRepository transactionRepo,
        IUnitOfWork uow)
    {
        _accountRepo = accountRepo;
        _transactionRepo = transactionRepo;
        _uow = uow;
    }

    public async Task<Result> Handle(TransferAmountCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _uow.BeginTransactionAsync();

        try
        {
            var sourceAccount = await _accountRepo.GetByIdAsync(request.SourceAccountId);
            var destinationAccount = await _accountRepo.GetByIdAsync(request.DestinationAccountId);

            // Validações
            if (sourceAccount == null || destinationAccount == null)
                return Result.Fail("Conta de origem/destino inválida.");

            if (!sourceAccount.HasSufficientBalance(request.Amount))
                return Result.Fail("Saldo insuficiente.");

            // Atualiza saldos
            sourceAccount.Withdraw(request.Amount);
            destinationAccount.Deposit(request.Amount);

            // Registra transação
            var transactionRecord = new Transaction
            {
                SourceAccountId = sourceAccount.Id,
                DestinationAccountId = destinationAccount.Id,
                Amount = request.Amount,
                Type = TransactionType.Transfer,
                Status = TransactionStatus.Completed
            };

            await _transactionRepo.InsertAsync(transactionRecord);
            await _uow.CommitAsync();
            await transaction.CommitAsync();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result.Fail($"Erro na transferência: {ex.Message}");
        }
    }
}