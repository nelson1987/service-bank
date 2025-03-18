using Junta.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Junta.UnitTests;

public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
        IAsyncLifetime
{
    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public Task DisposeAsync()
    {
        throw new NotImplementedException();
    }

    // private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
    //     .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
    //     .WithPassword("1q2w3e4r@#$!")
    //     .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
    //     .Build();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(_ =>
        {
            _.AddTransient<ICreateProductCommandHandler, CreateProductHandler>();
            _.AddTransient<IProductRepository, ProductRepository>();
            _.AddTransient<IDbContext, ProductContext>();
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
}