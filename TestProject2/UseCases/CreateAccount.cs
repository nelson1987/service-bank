using FluentAssertions;

namespace TestProject2.UseCases;

public static class Constantes
{
    public const string InvalidNumber = "email@email.com";
}

public class InvalidNumber : Exception
{
    public InvalidNumber(string message) : base(message)
    {
    }
}

public static class ContaExtensions
{
    public static Conta CriarConta(this Conta conta, string nome)
    {
        // if (string.IsNullOrEmpty(conta))
        conta.Numero = nome;
        return conta;
    }
}

public class Conta
{
    public string Numero { get; set; }
}
public class AccountRepository
{
    public void Insert(Conta conta)
    {
        throw new System.NotImplementedException();
    }

    public Conta Select(string accountNumber)
    {
        throw new System.NotImplementedException();
    }
}

public class AccountUnitTest
{
    [Fact]
    public void GivenAccountNameIsValid_WhenCreateAccount_ThenAccountIsCreated()
    {
        var contaCriada = new Conta()
            .CriarConta("123456");
        contaCriada.Should().NotBeNull();
        contaCriada.Numero.Should().Be("123456");
    }

    // [Fact]
    // public void GivenAccountNameIsInvalid_WhenCreateAccount_ThenRaisedException()
    // {
    //     var contaCriada = () => new Conta()
    //         .CriarConta(null);
    //     contaCriada.Should().Throw<ArgumentNullException>()
    //         .WithMessage("Value cannot be null. (Parameter 'numero')");
    // }
}

public class CreateAccountRepositoryUnitTest
{
    [Fact]
    public void GivenAccountDataIsValid_WhenCreateAccount_ThenAccountIsCreated()
    {
        var handler = new AccountRepository();
        var conta = new Conta().CriarConta("123456");
        handler.Insert(conta);
        var contaCriada = handler.Select("123456");
        contaCriada.Should().NotBeNull();
        contaCriada.Numero.Should().Be(conta.Numero);
    }
}