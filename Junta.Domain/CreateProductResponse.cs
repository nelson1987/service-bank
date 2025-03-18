namespace Junta.Domain;
public record CreateProductResponse(Guid Id, string Name, string Description, decimal Price, int Stock);