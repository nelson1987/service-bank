using AutoFixture;
using FluentAssertions;

namespace TestProject1.Basics;

public class BasicRequestUnitTests : BasicUnitTests
{
    private BasicRequest _request;

    public BasicRequestUnitTests()
    {
        _request = Fixture.Create<BasicRequest>();
    }

    [Fact]
    public void WithSuccess()
    {
        _request.Name.Should().NotBeNullOrEmpty();
        _request.Name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void WithErrorMessage()
    {
        _request = Fixture.Build<BasicRequest>()
            .With(x => x.Name, string.Empty)
            .Create();
        _request.Name.Should().BeNullOrEmpty();
        _request.Name.Should().BeNullOrWhiteSpace();
    }
}