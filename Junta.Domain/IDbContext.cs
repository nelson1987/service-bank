namespace Junta.Domain;

public interface IDbContext
{
    List<Product> Products { get; set; }
}