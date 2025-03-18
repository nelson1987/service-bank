namespace Junta.Domain;

public interface ICreateProductCommandHandler
{
    Task<Result<CreateProductResponse>> HandleAsync(CreateProductCommand id);
}