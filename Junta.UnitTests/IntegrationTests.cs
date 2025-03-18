using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace Junta.UnitTests;

public abstract class IntegrationTests
{
    protected readonly HttpClient Client;

    protected readonly IFixture Fixture = new Fixture()
        .Customize(new AutoMoqCustomization { ConfigureMembers = true });

    protected IntegrationTests()
    {
        var factory = new IntegrationTestWebAppFactory();
        Client = factory.CreateDefaultClient();
    }

    protected StringContent SetContent<T>(T command)
    {
        var commandSerialize = JsonSerializer.Serialize(command);
        return new StringContent(commandSerialize, Encoding.UTF8, "application/json");
    }

    protected T? GetContent<T>(HttpResponseMessage response)
    {
        return response.Content.ReadFromJsonAsync<T>().Result;
    }
}