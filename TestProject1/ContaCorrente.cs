// using AutoFixture;
// using AutoFixture.AutoMoq;
// using AutoFixture.Dsl;
// using FluentAssertions;
// using Moq;
//
// namespace TestProject1;
//
// public static class Constantes
// {
//     public const string SaldoInsuficiente = "Saldo insuficiente para essa operação.";
//     public const string ValorInvalido = "Valor invalido para essa operação.";
//     public const string ContaCorrenteNaoEncontrada = "Conta corrente não encontrada.";
// }
//
// public class ContaCorrente
// {
//     public ContaCorrente(string numeroConta)
//     {
//         NumeroConta = numeroConta ?? throw new ArgumentNullException(nameof(numeroConta));
//     }
//
//     public string NumeroConta { get; set; }
//     public decimal Saldo { get; set; }
//
//     /// <summary>
//     ///     Sacar
//     /// </summary>
//     /// <param name="valor"></param>
//     /// <exception cref="Exception"></exception>
//     public void Cashout(decimal valor)
//     {
//         if (Saldo <= 0.00M)
//             throw new Exception(Constantes.SaldoInsuficiente);
//         if (valor > Saldo)
//             throw new Exception(Constantes.SaldoInsuficiente);
//         Saldo -= valor;
//     }
//
//     /// <summary>
//     ///     Depositar
//     /// </summary>
//     /// <param name="valor"></param>
//     /// <exception cref="Exception"></exception>
//     public void Cashin(decimal valor)
//     {
//         if (valor <= 0.00M)
//             throw new Exception(Constantes.ValorInvalido);
//         Saldo += valor;
//     }
// }
//
// public class ContaCorrenteTests
// {
//     private readonly ContaCorrente _sut = new("123");
//
//     [Fact]
//     public void Dado_ContaCorrenteValida_Quando_RealizarSaque_Deve_MudarSaldo()
//     {
//         // Arrange
//         _sut.Saldo = 123.45M;
//         // Act
//         _sut.Cashout(0.01M);
//         // Assert
//         _sut.Saldo.Should().Be(123.44M);
//     }
//
//     [Fact]
//     public void Dado_ContaCorrenteSemFundo_Quando_RealizarSaque_Deve_RetornarMensagemSaldoInsuficiente()
//     {
//         // Arrange
//         _sut.Saldo = 0.00M;
//         // Act
//         var saque = () => _sut.Cashout(0.01M);
//         // Assert
//         saque.Should()
//             .ThrowExactly<Exception>()
//             .WithMessage(Constantes.SaldoInsuficiente);
//     }
//
//     [Fact]
//     public void Dado_ContaCorrenteComFundo_Quando_RealizarSaqueMaiorQueSaldo_Deve_RetornarMensagemSaldoInsuficiente()
//     {
//         // Arrange
//         _sut.Saldo = 123.45M;
//         // Act
//         var saque = () => _sut.Cashout(234.56M);
//         // Assert
//         saque.Should()
//             .ThrowExactly<Exception>()
//             .WithMessage(Constantes.SaldoInsuficiente);
//     }
//
//     [Fact]
//     public void Dado_ContaCorrenteValida_Quando_RealizarDeposito_Deve_MudarSaldo()
//     {
//         // Arrange
//         _sut.Saldo = 123.45M;
//         // Act
//         _sut.Cashin(0.01M);
//         // Assert
//         _sut.Saldo.Should().Be(123.46M);
//     }
//
//     [Fact]
//     public void Dado_ContaCorrenteValida_Quando_RealizarDepositoMenorOuIgualZero_Deve_RetornarMensagemValorInvalido()
//     {
//         // Arrange
//         _sut.Saldo = 123.45M;
//         // Act
//         var deposito = () => _sut.Cashin(0.00M);
//         // Assert
//         deposito.Should()
//             .ThrowExactly<Exception>()
//             .WithMessage(Constantes.ValorInvalido);
//     }
// }
//
// public interface IContaCorrenteRepository
// {
//     ContaCorrente? Get(string numeroConta);
// }
//
// public class PixCashinService
// {
//     private readonly IContaCorrenteRepository _repository;
//
//     public PixCashinService(IContaCorrenteRepository repository)
//     {
//         _repository = repository;
//     }
//
//     public ContaCorrente Execute(string numeroConta, decimal valor)
//     {
//         var conta = _repository.Get(numeroConta);
//         if (conta == null)
//             throw new Exception(Constantes.ContaCorrenteNaoEncontrada);
//         conta.Cashin(valor);
//         return conta;
//     }
// }
//
// public class PixCashinServiceTests
// {
//     private readonly ICustomizationComposer<ContaCorrente> _conta;
//     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
//     private readonly Mock<IContaCorrenteRepository> _repository;
//     private readonly PixCashinService _sut;
//
//
//     public PixCashinServiceTests()
//     {
//         _conta = _fixture.Build<ContaCorrente>();
//         _repository = _fixture.Freeze<Mock<IContaCorrenteRepository>>();
//         _sut = _fixture.Create<PixCashinService>();
//     }
//
//     [Fact]
//     public void Dado_PixCashinService_Quando_ContaEncontrada_Deve_RetornarContaModificada()
//     {
//         // Arrange
//         var contaEncontrada = _conta
//             .With(x => x.Saldo, 123.45M)
//             .Create();
//         _repository
//             .Setup(x => x.Get(It.IsAny<string>()))
//             .Returns(contaEncontrada);
//         // Act
//         var contaDebitada = _sut.Execute(It.IsAny<string>(), 0.01M);
//         // Assert
//         contaDebitada.Saldo.Should().Be(123.46M);
//     }
//
//     [Fact]
//     public void Dado_PixCashinService_Quando_ContaNaoEncontrada_Deve_RetornarMensagemContaNaoEncontrada()
//     {
//         // Arrange
//         _repository
//             .Setup(x => x.Get(It.IsAny<string>()))
//             .Returns((ContaCorrente)null!);
//         // Act
//         var conta = () => _sut.Execute(It.IsAny<string>(), 0.01M);
//         // Assert
//         conta.Should()
//             .ThrowExactly<Exception>()
//             .WithMessage(Constantes.ContaCorrenteNaoEncontrada);
//     }
// }