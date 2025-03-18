namespace Junta.Web.Controllers;

public interface ICreateProductCommandHandler
{
    Task<Result<CreateProductResponse>> HandleAsync(CreateProductCommand id);
}