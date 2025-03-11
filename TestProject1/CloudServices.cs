// using AutoFixture;
// using AutoFixture.AutoMoq;
// using FluentAssertions;
// using Moq;
//
// namespace TestProject1;
//
// public record FundCashinCommand
// {
//     public decimal Valor { get; set; }
// }
//
// public record FundCashin
// {
//     public Guid Id { get; set; }
//     public decimal Valor { get; set; }
// }
//
// public record FundCashinResponse
// {
//     public Guid Id { get; set; }
//     public decimal Valor { get; set; }
// }
//
// public interface IDatabaseService
// {
//     Task<FundCashin> InsertAsync(FundCashin entity);
// }
//
// public class FundCashinHandler
// {
//     private readonly IDatabaseService _databaseService;
//
//     public FundCashinHandler(IDatabaseService databaseService)
//     {
//         _databaseService = databaseService;
//     }
//
//     public async Task<FundCashinResponse> Handle(FundCashinCommand command)
//     {
//         var id = Guid.NewGuid();
//         await _databaseService.InsertAsync(new FundCashin { Id = id, Valor = command.Valor });
//         return new FundCashinResponse { Id = id, Valor = command.Valor };
//     }
// }
//
// public class MicroservicesUnitTests
// {
//     private readonly Mock<IDatabaseService> _database;
//     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
//     private readonly FundCashinHandler _sut;
//
//     public MicroservicesUnitTests()
//     {
//         _database = _fixture.Freeze<Mock<IDatabaseService>>();
//         _sut = _fixture.Create<FundCashinHandler>();
//     }
//
//     [Fact]
//     public async Task WithSuccess()
//     {
//         // Arrange
//         var fundCashin = _fixture.Create<FundCashin>();
//         _database.Setup(x => x.InsertAsync(It.Is<FundCashin>(x => x.Valor == fundCashin.Valor)))
//             .ReturnsAsync(fundCashin);
//         // _database.Setup(x => x.InsertAsync(It.IsAny<FundCashin>()))
//         //     .ReturnsAsync(fundCashin);
//         // Act
//         var result = await _sut.Handle(It.IsAny<FundCashinCommand>());
//         // Assert
//         result.Should().BeOfType<FundCashinResponse>();
//         result.Id.Should().Be(fundCashin.Id);
//         result.Valor.Should().Be(fundCashin.Valor);
//         _database.Verify(x => x.InsertAsync(It.IsAny<FundCashin>()), Times.Once);
//     }
// }