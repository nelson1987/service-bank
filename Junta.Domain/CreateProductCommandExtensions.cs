namespace Junta.Domain;

public static class CreateProductCommandExtensions
{
    public static Product MapToEntity(this CreateProductCommand product)
    {
        return new Product(
            product.Name,
            product.Description,
            product.Price,
            product.Stock);
    }
}