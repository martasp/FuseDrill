using GeneratedClientMyNamespace1;
using tests.Fuzzer;
namespace tests;

// Define a test collection
[CollectionDefinition("Sequential Tests", DisableParallelization = true)]
public class SequentialTestsCollection { }

[Collection("Sequential Tests")]
public class DrillerTests
{
    [Fact]
    public async Task TestMinimumConfiguration()
    {
        //if you are using  top level statements in web api you need to add :
        //public partial class Program { } in Program.cs class

        var fuzzer = new ApiFuzzerWithVerifier<Program>();
        await fuzzer.TestWholeApi();
    }

    [Fact]
    public async Task TestRemoteFuzzing()
    {
        // Search for "TestApi.csproj" starting from the current directory
        var apiProjectFileName = "TestApi.csproj";

        // Start the project using dotnet run
        var apiProcessManager = new ApiProcessManager();
        await apiProcessManager.DotnetRun(apiProjectFileName);

        var apiUrl = "http://localhost:5184/";
        var swaggerPath = "http://localhost:5184/swagger/v1/swagger.json";

        var newHttpClient = new HttpClient
        {
            BaseAddress = new Uri(apiUrl)
        };

        var fuzzer = new ApiFuzzerWithVerifier(newHttpClient, swaggerPath);
        await fuzzer.TestWholeApi();

        await apiProcessManager.DisposeAsync();
    }

    [Fact]
    public async Task LibraryConsumerProvidesTheOpenAPIClient()
    {
        var factory = new TestApplication<Program>();
        var httpClient = factory.CreateClient();
        var generatedClient = new GeneratedClient("http://localhost/", httpClient);
        var fuzzer = new ApiFuzzerWithVerifier(generatedClient);
        await fuzzer.TestWholeApi();
    }
}

