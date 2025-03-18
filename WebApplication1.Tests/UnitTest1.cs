
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
namespace WebApplication1.Tests;

public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
        IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // var descriptorType = typeof(DbContextOptions<ApplicationDbContext>);
            //
            // var descriptor = Enumerable
            //     .SingleOrDefault<ServiceDescriptor>(services, s => s.ServiceType == descriptorType);
            //
            // if (descriptor is not null) services.Remove(descriptor);
            //
            // EntityFrameworkServiceCollectionExtensions.AddDbContext<ApplicationDbContext>(services, options =>
            //     SqlServerDbContextOptionsExtensions.UseSqlServer(options, _dbContainer.GetConnectionString()));
        });
    }


    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }
}
public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        HttpClient _client = new HttpClient()
        {
        };
    }
}