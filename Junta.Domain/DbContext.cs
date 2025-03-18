namespace Junta.Domain;

public class DbContext : IDbContext
{
    public List<Product> Products { get; set; }
}