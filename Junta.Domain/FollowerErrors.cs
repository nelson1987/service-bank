namespace Junta.Domain;

public static class FollowerErrors
{
    public static readonly Error AlreadyFollowing = new Error(
        "Followers.AlreadyFollowing", "Already following");

    public static Error NotFound(Guid id) => new Error(
        "ContaDebitante.NotFound", $"The follower with Id '{id}' was not found");
    
    public static Error CreateProduct(Exception ex) => new Error(
        "Product.Create", $"TErro ao criar produto: {ex.Message}");
}