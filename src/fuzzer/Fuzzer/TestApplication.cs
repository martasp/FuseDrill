using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

//https://github.com/dotnet/AspNetCore.Docs.Samples/tree/main/fundamentals/minimal-apis/samples/MinApiTestsSample/IntegrationTests/Helpers
public class TestApplication<T> : WebApplicationFactory<T> where T : class
{
    private readonly string _environment;
    public TestApplication()
    {

        _environment = "Development";
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_environment);

        // Load configuration from the same appsettings.json as the main application
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
        });

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
        });

        return base.CreateHost(builder);
    }
}