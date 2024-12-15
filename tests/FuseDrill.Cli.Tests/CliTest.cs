//using Microsoft.VisualStudio.TestPlatform.TestHost;
//using FuseDrill.Core;
//using static HelperFunctions;
//using FuseDrill;
//using DotNet.Testcontainers.Builders;

//public class CliFlowTests
//{
//    //[Fact(Skip = "Test Uses LLM, only good for manual debugging and tweaking prompt")]
//    [Fact]
//    public async Task CompareFuzzingsWithLLMTest()
//    {
//        var envars = Environment.GetEnvironmentVariables();

//        var owner = envars["GITHUB_REPOSITORY_OWNER"]?.ToString(); // e.g., "owner"
//        var repoName = envars["GITHUB_REPOSITORY"]?.ToString()?.Split('/')?[1]; // e.g., "repo-name"
//        var branch = envars["GITHUB_HEAD_REF"]?.ToString(); // e.g., "refs/heads/branch-name"
//        var githubToken = envars["GITHUB_TOKEN"]?.ToString();
//        var fuseDrillBaseAddres = (envars["FUSEDRILL_BASE_ADDRESS"]?.ToString());
//        var fuseDrillOpenApiUrl = (envars["FUSEDRILL_OPENAPI_URL"]?.ToString());
//        var fuseDrillTestAccountOAuthHeaderValue = envars["FUSEDRILL_TEST_ACCOUNT_OAUTH_HEADER_VALUE"]?.ToString();
//        var smokeFlag = envars["SMOKE_FLAG"]?.ToString() == "true";


//        var dockerImageUrl = "fusedrill/testapi:latest"; // Change this to your actual image URL
//        var containerName = "testapi";
//        fuseDrillBaseAddres = "http://localhost:8080/"; // Ensure this reflects the exposed port correctly
//        fuseDrillOpenApiUrl = "http://localhost:8080/swagger/v1/swagger.json"; // Ensure this reflects the exposed port correctly

//        // Set up a TestContainer for the image
//        var containerBuilder = new ContainerBuilder()
//            .WithImage(dockerImageUrl)
//            .WithName(containerName)
//            .WithPortBinding(8080, 8080) // Host:Container port mapping (Exposing port 8080)
//            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

//        var container = containerBuilder.Build();
//        await container.StartAsync();

//        try
//        {

//#if DEBUG
//            githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
//            branch = "test2";
//            repoName = "FuseDrill";
//            owner = "martasp";
//#endif

//            await CliFlow(owner, repoName, branch, githubToken, fuseDrillBaseAddres, fuseDrillOpenApiUrl, fuseDrillTestAccountOAuthHeaderValue, smokeFlag);

//        }
//        finally
//        {
//            // Stop and remove the container after the test
//            await container.StopAsync();
//            await container.DisposeAsync();
//        }

//    }
//}
