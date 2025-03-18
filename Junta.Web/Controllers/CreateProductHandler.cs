namespace Junta.Web.Controllers;

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
            var product = new Product(command.Name, command.Description, command.Price, command.Stock);
            await _productRepository.AddAsync(product);
            return Result.Success(product.MapToResponse());
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<CreateProductResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<CreateProductResponse>($"Erro ao criar produto: {ex.Message}");
        }
    }
}