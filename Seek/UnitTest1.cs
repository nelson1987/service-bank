namespace Seek;

public record Responsavel(string Nome);
public record Cliente(string Nome);
public record Produto(string Nome);
public class Pedido
{
    public Pedido(Cliente comprador, Responsavel responsavel)
    {
        Protocolo = Guid.NewGuid().ToString();
        Comprador = comprador;
        Responsavel = responsavel;
        Produtos = new List<Produto>();
    }

    public string Protocolo { get; private set; }
    public Cliente Comprador { get; private set; }
    public Responsavel Responsavel { get; private set; }
    public List<Produto> Produtos { get; private set; }

    public void AdicionarProduto(Produto produto)
    {
        Produtos.Add(produto);
    }
};
public record ListagemPedido(string Produto, int Quantidade);
public record CompradorPedidoResponse(string Comprador, string Responsavel);
public record ProdutoPedidoResponse(string Nome);
public record PedidoResponse(string Protocolo, ProdutoPedidoResponse Produto, CompradorPedidoResponse Comprador);
public class ResponsavelUnitTest
{
    [Fact]
    public void Criar_Responsavel()
    {
        var responsavel = new Responsavel("João");
        Assert.Equal("João", responsavel.Nome);
    }
}
public class ClienteUnitTest
{
    [Fact]
    public void Criar_Cliente()
    {
        var cliente = new Cliente("Condominio");
        Assert.Equal("Condominio", cliente.Nome);
    }
}
public class ProdutoUnitTest
{
    [Fact]
    public void Criar_Produto()
    {
        var produto = new Produto("Tomate");
        Assert.Equal("Tomate", produto.Nome);
    }
}
public class PedidoUnitTest
{
    [Fact]
    public void Criar_Pedido()
    {
        var responsavel = new Responsavel("João");
        var cliente = new Cliente("Condominio");
        var produto = new Produto("Tomate");
        var pedido = new Pedido(cliente, responsavel);
        pedido.AdicionarProduto(produto);
        Assert.NotEqual(Guid.NewGuid().ToString(), pedido.Protocolo);
        Assert.Equal("Tomate", pedido.Produtos[0].Nome);
        Assert.Equal("Condominio", pedido.Comprador.Nome);
        Assert.Equal("João", pedido.Responsavel.Nome);
    }
}
public class CompradorPedidoResponseUnitTest
{
    [Fact]
    public void Criar_CompradorPedidoResponse()
    {
        var responsavel = new Responsavel("João");
        var cliente = new Cliente("Condominio");
        var compradorPedidoResponse = new CompradorPedidoResponse(cliente.Nome, responsavel.Nome);
        Assert.Equal("Condominio", compradorPedidoResponse.Comprador);
        Assert.Equal("João", compradorPedidoResponse.Responsavel);
    }
}
public class ProdutoPedidoResponseUnitTest
{
    [Fact]
    public void Criar_ProdutoPedidoResponse()
    {
        var produto = new Produto("Tomate");
        var produtoPedidoResponse = new ProdutoPedidoResponse(produto.Nome);
        Assert.Equal("Tomate", produtoPedidoResponse.Nome);
    }
}
public class PedidoResponseUnitTest
{
    [Fact]
    public void PedidoResponse_Quando_DadosValidos_RetornaSucesso()
    {
        var responsavel = new Responsavel("João");
        var cliente = new Cliente("Condominio");
        var produto = new Produto("Tomate");
        var pedido = new Pedido(cliente, responsavel);
        pedido.AdicionarProduto(produto);
        var produtoPedidoResponse = new ProdutoPedidoResponse(produto.Nome);
        var compradorPedidoResponse = new CompradorPedidoResponse(cliente.Nome, responsavel.Nome);
        var resultado = new PedidoResponse(pedido.Protocolo, produtoPedidoResponse, compradorPedidoResponse);
        Assert.Equal("João", resultado.Comprador.Responsavel);
        Assert.Equal("Condominio", resultado.Comprador.Comprador);
        Assert.Equal("Tomate", resultado.Produto.Nome);
    }
}
public class PedidoService
{
    public static List<ListagemPedido> Listar(List<Pedido> pedidos)
    {
        return pedidos
            .GroupBy(x => x.Produtos)
            .Select(x => new ListagemPedido(x.First().Produtos[0].Nome, x.Count()))
            .ToList();
    }
}
public class PedidoServiceUnitTest
{
    [Fact]
    public void Pedido_Quando_DadosValidos_RetornaSucesso()
    {
        var responsavel = new Responsavel("João");
        var cliente = new Cliente("Condominio");
        var produto = new Produto("Tomate");
        var pedido = new Pedido(cliente, responsavel);
        pedido.AdicionarProduto(produto);
        var manuel = new Responsavel("Manuel");
        var vila = new Cliente("Vila");
        var agriao = new Produto("Agrião");
        var pedido2 = new Pedido(vila, manuel);
        pedido2.AdicionarProduto(agriao);
        var radial = new Cliente("Radial");
        var pedido3 = new Pedido(radial, manuel);
        pedido3.AdicionarProduto(agriao);

        var pedidoList = new List<Pedido> { pedido, pedido2, pedido3 };
        var listagem = PedidoService.Listar(pedidoList);
        
        Assert.NotEqual(responsavel, manuel);
        Assert.NotEqual(cliente, vila);
        Assert.NotEqual(agriao, produto);
        Assert.NotEqual(pedido, pedido2);
        Assert.Equal(3, pedidoList.Count);
        Assert.Equal("Tomate", listagem[0].Produto);
        Assert.Equal(1, listagem[0].Quantidade);
        Assert.Equal("Agrião", listagem[1].Produto);
        Assert.Equal(1, listagem[1].Quantidade);
    }

    [Fact]
    public void PedidoResponse_PodeTerOMesmoResponsavel()
    {
        var responsavel = new Responsavel("João");
        var cliente = new Cliente("Condominio");
        var produto = new Produto("Tomate");
        var pedido = new Pedido(cliente, responsavel);
        pedido.AdicionarProduto(produto);
        var vila = new Cliente("Vila");
        var agriao = new Produto("Agrião");
        var pedido2 = new Pedido(vila, responsavel);
        pedido2.AdicionarProduto(agriao);

        var pedidoList = new List<Pedido> { pedido, pedido2 };
        var listagem = PedidoService.Listar(pedidoList);

        Assert.Equal(pedido.Responsavel, pedido2.Responsavel);
        Assert.NotEqual(cliente, vila);
        Assert.NotEqual(agriao, produto);
        Assert.NotEqual(pedido, pedido2);
        Assert.Equal("Tomate", listagem[0].Produto);
        Assert.Equal(1, listagem[0].Quantidade);
        Assert.Equal("Agrião", listagem[1].Produto);
        Assert.Equal(1, listagem[1].Quantidade);
    }
}

/*
 * Logar
 * Selecionar Produto
 * Comprar Produto
 * Cadastrar Cliente
 * O Fornecedor recebendo o pedido
 * Colocar numa onda
 * Faturar Pedido
 * Retornar a nota para pagamento

 * Eu Nelson em nome do Condomínio Flor do Campo logo no sistema
 * Eu Seleciono os produtos que comprarei
 * Finalizo o pedido
 * O Pedido é enviado ao Administrador do sistema, que está aguardando o fechamento dos lotes
 * A cada fechamento de lote por produto o cliente será notificado
 * O fornecedor faturará o pedido(gerará uma nota fiscal para o cliente)
 * Após a confirmação do pagamento pelo cliente
 * O fornecedor indicará a data prevista da entrega
 */