using FluentAssertions;
using Junta.Web.Controllers;

namespace Junta.UnitTests;

public class FollowerErrorsUnitTests : UnitTests
{
    [Fact]
    public void Validar_FollowerErrors_AlreadyFollowing()
    {
        var errorFact = FollowerErrors.AlreadyFollowing;
        errorFact.Should().BeOfType<Error>();
        errorFact.Code.Should().Be("Followers.AlreadyFollowing");
        errorFact.Description.Should().Be("Already following");
    }
    
    [Fact]
    public void Validar_FollowerErrors_NotFound()
    {
        var guidId = Guid.NewGuid();
        var errorFact = FollowerErrors.NotFound(guidId);
        errorFact.Should().BeOfType<Error>();
        errorFact.Code.Should().Be("ContaDebitante.NotFound");
        errorFact.Description.Should().Be($"The follower with Id '{guidId}' was not found");
    }
}