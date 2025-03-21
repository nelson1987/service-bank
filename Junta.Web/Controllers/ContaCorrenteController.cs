using Junta.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Junta.Web.Controllers;

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
        try
        {
            var result = await createProductCommandHandler.HandleAsync(command);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        throw new NotImplementedException();
    }
}