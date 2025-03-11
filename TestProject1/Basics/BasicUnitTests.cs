using AutoFixture;
using AutoFixture.AutoMoq;

namespace TestProject1.Basics;

public class BasicUnitTests
{
    protected readonly IFixture Fixture = new Fixture()
        .Customize(new AutoMoqCustomization
        {
            ConfigureMembers = true
        });
}