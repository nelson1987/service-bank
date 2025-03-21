using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AutoFixture;
using AutoFixture.AutoMoq;
using Junta.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Junta.UnitTests;

public abstract class IntegrationTests : IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly IProductRepository ProductRepository;
    protected readonly IDbContext DbContext;
    private readonly IServiceScope _scope;

    protected readonly IFixture Fixture = new Fixture()
        .Customize(new AutoMoqCustomization { ConfigureMembers = true });

    protected IntegrationTests()
    {
        var factory = new IntegrationTestWebAppFactory();
        Client = factory.CreateDefaultClient();
        _scope = factory.Services.CreateScope();
        ProductRepository = _scope.ServiceProvider.GetRequiredService<IProductRepository>();
        DbContext = _scope.ServiceProvider.GetRequiredService<IDbContext>();
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

    public Task InitializeAsync()
    {
        // return DeleteAllProductsAsync();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        // return DeleteAllProductsAsync();
        return Task.CompletedTask;
    }

    private async Task DeleteAllProductsAsync()
    {
        var produtos = await ProductRepository.GetAllAsync();
        foreach (var produto in produtos)
        {
            await ProductRepository.DeleteAsync(produto.Id);
        }
        _scope.Dispose();
        // DbContext
    }
}