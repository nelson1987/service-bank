namespace Junta.Web.Controllers;

public record CreateProductCommand(string Name, string Description, decimal Price, int Stock);