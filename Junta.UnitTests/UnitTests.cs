using AutoFixture;
using AutoFixture.AutoMoq;

namespace Junta.UnitTests;

public abstract class UnitTests
{
    protected readonly IFixture Fixture = new Fixture()
        .Customize(new AutoMoqCustomization { ConfigureMembers = true });
}