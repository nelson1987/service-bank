namespace Junta.Tests;
// public record CreatePedidoCommand(string Nome);
// public record CreatePedidoResponse(string Nome);
// public class CreatePedidoValidator : AbstractValidator<CreatePedidoCommand>
// {
//     public CreatePedidoValidator()
//     {
//         RuleFor(x => x.Nome)
//             .NotEmpty()
//             .NotNull();
//     }
// }
// public class Handler
// {
//     private readonly IValidator<CreatePedidoCommand> _pedidoValidator;
//     private readonly IUnitOfWork _unitOfWork;
//
//     public Handler(IValidator<CreatePedidoCommand> pedidoValidator,
//         IUnitOfWork unitOfWork)
//     {
//         _pedidoValidator = pedidoValidator;
//         _unitOfWork = unitOfWork;
//     }
//
//     public CreatePedidoResponse Handle(CreatePedidoCommand command)
//     {
//         _unitOfWork.BeginTransaction();
//         try
//         {
//             ValidationResult? validator = _pedidoValidator.Validate(command);
//             if (validator == null)
//                 throw new NotImplementedException();
//             if (!validator.IsValid)
//                 throw new NotImplementedException();
//
//             var pedido = _unitOfWork.Pedidos.FindByName(command.Nome);
//             if (pedido == null)
//                 throw new NotImplementedException();
//
//             _unitOfWork.Pedidos.Adicionar(pedido);
//             _unitOfWork.Commit();
//
//             return new CreatePedidoResponse(command.Nome);
//         }
//         catch (Exception ex)
//         {
//             _unitOfWork.Rollback();
//             throw ex;
//         }
//     }
// }
// public class PostUnitTest
// {
//     private readonly IFixture _fixture = new Fixture()
//         .Customize(new AutoMoqCustomization { ConfigureMembers = true });
//
//     private readonly CreatePedidoCommand _command;
//     private readonly CreatePedidoResponse _response;
//     private readonly Handler _handler;
//     private readonly Mock<IValidator<CreatePedidoCommand>> _validatorMock;
//
//     public PostUnitTest()
//     {
//         _validatorMock = _fixture.Freeze<Mock<IValidator<CreatePedidoCommand>>>();
//         _command = _fixture.Create<CreatePedidoCommand>();
//         _validatorMock
//             .Setup(x => x.Validate(_command))
//             .Returns(new ValidationResult());
//         _handler = _fixture.Create<Handler>();
//     }
//
//     [Fact]
//     public void Post_Pedido()
//     {
//         var response = _handler.Handle(_command);
//         response.Nome.Should().Be(_command.Nome);
//     }
//
//     [Fact]
//     public void Post_PedidoValidation()
//     {
//         var response = () => _handler.Handle(_command with { Nome = string.Empty });
//         response.Should().Throw<NotImplementedException>();
//     }
// }
// public class Pedido
// {
//     public Guid Id { get; set; }
//     public string Nome { get; set; }
// }
// public interface IDbContext
// {
//     List<Pedido> Pedido { get; }
//     void SaveChanges();
// }
// public interface IPedidoRepository
// {
//     void Adicionar(Pedido pedido);
//     Pedido? FindByName(string nome);
// }
// public interface IUnitOfWork
// {
//     IPedidoRepository Pedidos { get; }
//     void BeginTransaction();
//     void Commit();
//     void Rollback();
// }
// public interface IPedidoService
// {
//     void Adicionar(Pedido pedido);
// }
// public class PedidoRepository : IPedidoRepository
// {
//     private readonly IDbContext _dbContext;
//
//     public PedidoRepository(IDbContext dbContext)
//     {
//         _dbContext = dbContext;
//     }
//
//     public void Adicionar(Pedido pedido)
//     {
//         try
//         {
//             _dbContext.Pedido.Add(pedido);
//             _dbContext.SaveChanges();
//         }
//         catch (Exception ex)
//         {
//             throw ex;
//         }
//     }
//
//     public Pedido? FindByName(string nome)
//     {
//         try
//         {
//             return _dbContext.Pedido.FirstOrDefault(x => x.Nome.Equals(nome));
//         }
//         catch (Exception ex)
//         {
//             throw ex;
//         }
//     }
// }
// public class PedidoService : IPedidoService
// {
//     private readonly IUnitOfWork _unitOfWork;
//
//     public PedidoService(IUnitOfWork unitOfWork)
//     {
//         _unitOfWork = unitOfWork;
//     }
//
//     public void Adicionar(Pedido pedido)
//     {
//         _unitOfWork.BeginTransaction();
//         try
//         {
//             _unitOfWork.Pedidos.Adicionar(pedido);
//             _unitOfWork.Commit();
//         }
//         catch (Exception ex)
//         {
//             _unitOfWork.Rollback();
//             throw ex;
//         }
//     }
// }
// public class PedidoUnitTest
// {
//     [Fact]
//     public void Post_Pedido()
//     {
//         var pedido = new Pedido();
//     }
// }
// public class PedidoRepositoryUnitTest
// {
//     private readonly IDbContext _context;
//
//     public PedidoRepositoryUnitTest(IDbContext context)
//     {
//         _context = context;
//     }
//
//     [Fact]
//     public void PedidoRepository_Adicionar()
//     {
//         var repository = new PedidoRepository(_context);
//         var pedido = new Pedido();
//         repository.Adicionar(pedido);
//     }
// }