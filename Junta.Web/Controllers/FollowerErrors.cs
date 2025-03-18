namespace Junta.Web.Controllers;

public static class FollowerErrors
{
    public static readonly Error AlreadyFollowing = new Error(
        "Followers.AlreadyFollowing", "Already following");

    public static Error NotFound(Guid id) => new Error(
        "ContaDebitante.NotFound", $"The follower with Id '{id}' was not found");
}