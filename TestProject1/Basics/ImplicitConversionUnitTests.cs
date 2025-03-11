using AutoFixture;
using FluentAssertions;

namespace TestProject1.Basics;

public class ImplicitConversionUnitTests : BasicUnitTests
{
    private readonly BasicRequest _request;

    public ImplicitConversionUnitTests()
    {
        _request = Fixture.Create<BasicRequest>();
    }

    [Fact]
    public void WithSuccess_Implicit_Conversion()
    {
        Entity entity = _request;
        entity.Id.Should().NotBeEmpty();
        entity.Name.Should().Be(_request.Name);
    }
}