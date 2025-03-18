namespace Junta.Web.Controllers;

public class Product
{
    private Product()
    {
    } // Para o ORM

    public Product(string name, string description, decimal price, int stock)
    {
        Id = Guid.NewGuid();
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetStock(stock);
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do produto não pode ser vazio", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Nome do produto não pode ter mais de 100 caracteres", nameof(name));

        Name = name;
    }

    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Descrição do produto não pode ser vazia", nameof(description));

        Description = description;
    }

    public void SetPrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Preço do produto deve ser maior que zero", nameof(price));

        Price = price;
    }

    public void SetStock(int stock)
    {
        if (stock < 0)
            throw new ArgumentException("Estoque não pode ser negativo", nameof(stock));

        Stock = stock;
    }
}