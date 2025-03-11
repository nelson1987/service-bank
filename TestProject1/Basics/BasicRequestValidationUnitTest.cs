using AutoFixture;
using FluentAssertions;

namespace TestProject1.Basics;

public class BasicRequestValidationUnitTest : BasicUnitTests
{
    private readonly BasicRequestValidation _sut = new();

    [Fact]
    public async Task WithSuccess()
    {
        var req = Fixture.Create<BasicRequest>();
        var result = await _sut.ValidateAsync(req);
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task WithError()
    {
        var req = Fixture.Build<BasicRequest>()
            .With(x => x.Name, string.Empty)
            .Create();
        var result = await _sut.ValidateAsync(req);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}