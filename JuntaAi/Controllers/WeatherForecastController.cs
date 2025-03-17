using Microsoft.AspNetCore.Mvc;

namespace JuntaAi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        var user = new Usuario("joao");
        var cliente = new Cliente("Condominio");
        var produto = new Produto("tomate");
        var produtoPedidoResponse = new ProdutoPedidoResponse(produto.Nome);
        var pedido = new Pedido(produto);
        var resultado = new PedidoResponse(pedido.Protocolo, produtoPedidoResponse);
        return Ok(resultado);
    }
}

public record PedidoCommand();

public record PedidoResponse(string Protocolo, ProdutoPedidoResponse Produtos);

public record ProdutoPedidoResponse(string Nome);

public class Usuario
{
    public Usuario(string email)
    {
        Email = email;
    }

    public string Email { get; private set; }
}

public class Cliente
{
    public Cliente(string nome)
    {
        Nome = nome;
    }

    public string Nome { get; private set; }
}

public class Produto
{
    public Produto(string nome)
    {
        Nome = nome;
    }

    public string Nome { get; private set; }
}

public class Pedido
{
    public Pedido(Produto produto)
    {
        Produto = produto;
        Protocolo = Guid.NewGuid().ToString();
    }

    public string Protocolo { get; private set; }

    public Produto Produto { get; private set; }
}