namespace Junta.Web.Controllers;

public class ProductContext : IDbContext
{
    public List<Product> Products { get; set; }
}