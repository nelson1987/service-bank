using Microsoft.AspNetCore.Mvc;

namespace Junta.Web.Controllers;

public class Result<T> : Result
{
    protected internal Result(T value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T Value { get; }
}

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
    public async Task<IActionResult> Post([FromServices] ICreateProductCommandHandler createProductCommandHandler,
        CreateProductCommand command)
    {
        var result = await createProductCommandHandler.HandleAsync(command);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        throw new NotImplementedException();
    }
}