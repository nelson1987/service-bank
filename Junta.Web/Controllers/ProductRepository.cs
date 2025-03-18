namespace Junta.Web.Controllers;

public class ProductRepository : IProductRepository
{
    private readonly IDbContext _context;

    public ProductRepository(IDbContext context)
    {
        _context = context;
    }

    public Task<Product> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
    }

    public Task UpdateAsync(Product product)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}