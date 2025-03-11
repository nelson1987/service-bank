using AutoFixture;
using FluentAssertions;

namespace TestProject1.Basics;

public class BasicResponseUnitTests : BasicUnitTests
{
    private readonly BasicEndpoint _handler;
    private readonly BasicRequest _request;

    public BasicResponseUnitTests()
    {
        _request = Fixture.Create<BasicRequest>();
        _handler = Fixture.Create<BasicEndpoint>();
    }

    [Fact]
    public void WithSuccess()
    {
        var result = _handler.Post(_request);
        result.Should().BeOfType<BasicResponse>();
        result.Id.Should().NotBeEmpty();
    }
}