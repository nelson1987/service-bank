using Microsoft.AspNetCore.Mvc;
using ToolBank.Tests.Libs;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContaCorrenteController : ControllerBase
{
    private readonly ILogger<ContaCorrenteController> _logger;

    public ContaCorrenteController(ILogger<ContaCorrenteController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromServices] ICreateContaCorrenteHandler handler,
        [FromBody] CreateResultadoCommand command)
    {
        _logger.LogInformation("Create Contacorrente | Post | {0}", command);
        var result = await handler.HandleAsync(command);
        if (result.IsFailed)
            return BadRequest(result.Errors);
        _logger.LogInformation("Conta corrente criado com sucesso!");
        return Created(result.Value.Id, result);
    }
}

public static class ContaCorrenteControllerExtensions
{
    public static IServiceCollection AddContaCorrenteServices(this IServiceCollection services)
    {
        services.AddTransient<ICreateContaCorrenteHandler, CreateContaCorrenteHandler>();
        return services;
    }
}

public interface ICreateContaCorrenteHandler : IBaseHandler<CreateResultadoCommand, CreateResultadoResponse>
{
}

public class CreateContaCorrenteHandler : BaseHandler<CreateResultadoCommand, CreateResultadoResponse>,
    ICreateContaCorrenteHandler
{
    private readonly ILogger<CreateContaCorrenteHandler> _logger;
    private readonly IContaRepository _contaRepository;

    public CreateContaCorrenteHandler(ILogger<CreateContaCorrenteHandler> logger, IContaRepository contaRepository)
    {
        _logger = logger;
        _contaRepository = contaRepository;
    }

    public override async Task<HandlerResult<CreateResultadoResponse>> HandleAsync(CreateResultadoCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Create CreateResultado | Post | {0}", command);
        if (command.Nome.Equals("Fail"))
            return Failure();

        var conta = await _contaRepository.GetByName(command.Nome);
        if (command.Nome.Equals("NotFound"))
            return NotFound(conta.Id);

        _logger.LogInformation("Conta corrente criado com sucesso!");
        return Success(new CreateResultadoResponse(conta.Id.ToString(), "Nome do Usuario"));
    }
}

public class ContaCorrente
{
    public Guid Id { get; set; }
}

public interface IContaRepository
{
    Task<ContaCorrente>? GetByName(string name);
}