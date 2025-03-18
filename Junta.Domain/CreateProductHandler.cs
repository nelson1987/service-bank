namespace Junta.Domain;

public class CreateProductHandler : ICreateProductCommandHandler
{
    private readonly IProductRepository _productRepository;

    public CreateProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<CreateProductResponse>> HandleAsync(CreateProductCommand command)
    {
        try
        {
            Product product = command.MapToEntity();
            await _productRepository.AddAsync(product);
            return Result.Success(product.MapToResponse());
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<CreateProductResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<CreateProductResponse>(FollowerErrors.CreateProduct(ex).Description);
        }
    }
}