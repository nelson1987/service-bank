namespace Junta.Web.Controllers;

public static class CreateProductResponseExtensions
{
    public static CreateProductResponse MapToResponse(this Product product)
    {
        return new CreateProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock);
    }
}