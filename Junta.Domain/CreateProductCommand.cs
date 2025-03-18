namespace Junta.Domain;

public record CreateProductCommand(string Name, string Description, decimal Price, int Stock);