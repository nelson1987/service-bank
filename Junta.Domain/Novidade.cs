namespace Junta.Domain.Novidade;

public class ContaCorrente
{
    public Guid Id { get; set; }
    public decimal Saldo { get; protected set; }
}

public abstract class Debitante : ContaCorrente
{
    public void Sacar(decimal valor)
    {
        Saldo -= valor;
    }
}

public abstract class Creditante : ContaCorrente
{
    public void Depositar(decimal valor)
    {
        Saldo += valor;
    }
}

public class Transferencia
{
    public Transferencia(ContaCorrente debitante, ContaCorrente creditante, decimal valor)
    {
        Id = Guid.NewGuid();
        Debitante = debitante;
        Creditante = creditante;
        Valor = valor;
    }

    public Guid Id { get; set; }
    public ContaCorrente Debitante { get; set; }
    public ContaCorrente Creditante { get; set; }
    public decimal Valor { get; set; }
}

public record TransferenciaDto(Guid Id, Guid Debitante, Guid Creditante, decimal Valor);

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

public static class Errors
{
    public static Error ValorMenorQueZero => new Error("", "Valor menor que zero");
    public static Error SaldoInsuficiente => new Error("", "Saldo insuficiente");
    public static Error DebitanteNaoEncontrado => new Error("", "Debitante não encontrada");
    public static Error CreditanteNaoEncontrado => new Error("", "Creditante não encontrado");

    public static Error NotFound(Guid id) => new Error(
        "ContaDebitante.NotFound", $"The follower with Id '{id}' was not found");

    public static Error CreateProduct(Exception ex) => new Error(
        "Product.Create", $"TErro ao criar produto: {ex.Message}");
}

public interface IContaCorrenteRepository
{
    Task<ContaCorrente?> ObterPorIdAsync(Guid id);
}

public interface ITransferenciaRepository
{
    Task AdicionarAsync(Transferencia transferencia);
}

public abstract record TransferenciaCommand(Guid DebitanteId, Guid CreditanteId, decimal Valor);

public class TransferenciaService(
    IContaCorrenteRepository contaCorrenteRepository,
    ITransferenciaRepository transferenciaRepository)
{
    public async Task<Error> Transferir(TransferenciaCommand command)
    {
        if (command.Valor < 0)
            return Errors.ValorMenorQueZero;

        if (await contaCorrenteRepository.ObterPorIdAsync(command.DebitanteId) is not Debitante debitante)
            return Errors.DebitanteNaoEncontrado;

        if (debitante.Saldo < command.Valor)
            return Errors.SaldoInsuficiente;

        if (await contaCorrenteRepository.ObterPorIdAsync(command.CreditanteId) is not Creditante creditante)
            return Errors.CreditanteNaoEncontrado;

        debitante.Sacar(command.Valor);
        creditante.Depositar(command.Valor);

        var transferencia = new Transferencia(debitante, creditante, command.Valor);
        await transferenciaRepository.AdicionarAsync(transferencia);
        return Error.None;
    }
}