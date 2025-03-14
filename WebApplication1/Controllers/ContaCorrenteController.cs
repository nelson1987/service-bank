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
    public override Task<HandlerResult<CreateResultadoResponse>> HandleAsync(CreateResultadoCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Nome.Equals("Fail"))
            return Task.FromResult(Failure());
        if (command.Nome.Equals("NotFound"))
            return Task.FromResult(NotFound(Guid.NewGuid()));
        return Task.FromResult(Ok(new CreateResultadoResponse("Id", "Nome do Usuario")));
    }
}