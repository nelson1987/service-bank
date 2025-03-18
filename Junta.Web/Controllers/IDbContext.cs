namespace Junta.Web.Controllers;

public interface IDbContext
{
    List<Product> Products { get; set; }
}