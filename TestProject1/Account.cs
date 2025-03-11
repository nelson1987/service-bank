// using AutoFixture;
// using AutoFixture.AutoMoq;
// using FluentAssertions;
// using Moq;
//
// namespace TestProject1;
//
// //Command
// public class CreateCashoutCommand
// {
//     public string AccountNumber { get; set; }
// }
//
// //Respose
// public class CreateCashoutResponse
// {
//     public Guid Id { get; set; }
//     public string AccountNumber { get; set; }
// }
//
// //Event
// public class CreatedCashoutEvent
// {
//     public Guid Id { get; set; }
//     public string AccountNumber { get; set; }
// }
//
// //Handler
// public class Handler
// {
//     private readonly IProducer _produce;
//
//     public Handler(IProducer produce)
//     {
//         _produce = produce;
//     }
//
//     public CreateCashoutResponse Handle(CreateCashoutCommand createCashoutCommand)
//     {
//         _produce.Send(new CreatedCashoutEvent());
//         return new CreateCashoutResponse { AccountNumber = "12345678" };
//     }
// }
//
// public interface IProducer
// {
//     void Send(CreatedCashoutEvent @event);
// }
//
// #region Tests
//
// public class MockUnitTests
// {
//     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
//     private readonly Mock<IProducer> _producer;
//     private readonly Handler _sut;
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
//             .Setup(library => library.Send(It.IsAny<CreatedCashoutEvent>()));
//         //.Returns(@"c:\temp\test.txt");
//         var valor = _sut.Handle(new CreateCashoutCommand());
//         valor.AccountNumber.Should().Be("12345678");
//         _producer.Verify(library => library.Send(It.IsAny<CreatedCashoutEvent>()), Times.Once);
//     }
//
//     // [Fact]
//     // public void WithErrors()
//     // {
//     //     _producer
//     //         .Setup(library => library.FileExists("ping"))
//     //         .Throws(new Exception("When doing operation X, the service should be pinged always"));
//     //     var valor = () => _sut.DownloadExists(@"ping");
//     //     valor.Should()
//     //         .ThrowExactly<Exception>()
//     //         .WithMessage("When doing operation X, the service should be pinged always");
//     //     _producer.Verify(library => library.FileExists(It.IsAny<string>()), Times.Once);
//     // }
// }
//
// #endregion