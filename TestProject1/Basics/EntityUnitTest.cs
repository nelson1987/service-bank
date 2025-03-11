using AutoFixture;
using FluentAssertions;

namespace TestProject1.Basics;

public class EntityUnitTest : BasicUnitTests
{

    [Fact]
    public void WithSuccess()
    {
        var entity = Fixture.Create<Entity>();
        entity.Should().NotBeNull();
        entity.Should().BeOfType<Entity>();
        entity.Id.Should().NotBeEmpty();
    }
}