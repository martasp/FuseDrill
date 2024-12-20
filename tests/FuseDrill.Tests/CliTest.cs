using Microsoft.VisualStudio.TestPlatform.TestHost;
using FuseDrill.Core;
using static HelperFunctions;
using FuseDrill;
using DotNet.Testcontainers.Builders;

public class CliFlowTests
{
    [Fact(Skip = "Test uses LLM, only good for testing cli as a github environment")]
    //[Fact]
    public async Task CompareFuzzingsWithLLMTest()
    {
        var envars = Environment.GetEnvironmentVariables();
        var owner = envars["GITHUB_REPOSITORY_OWNER"]?.ToString(); // e.g., "owner"
        var repoName = envars["GITHUB_REPOSITORY"]?.ToString()?.Split('/')?[1]; // e.g., "repo-name"
        var branch = envars["GITHUB_HEAD_REF"]?.ToString(); // e.g., "refs/heads/branch-name"
        var githubToken = envars["GITHUB_TOKEN"]?.ToString();
        var fuseDrillBaseAddres = (envars["FUSEDRILL_BASE_ADDRESS"]?.ToString());
        var fuseDrillOpenApiUrl = (envars["FUSEDRILL_OPENAPI_URL"]?.ToString());
        var fuseDrillTestAccountOAuthHeaderValue = envars["FUSEDRILL_TEST_ACCOUNT_OAUTH_HEADER_VALUE"]?.ToString();
        var smokeFlag = envars["SMOKE_FLAG"]?.ToString() == "true";
        var pullReqestNumber = envars["GITHUB_REF_NAME"]?.ToString()?.Split('/')?[0]; // e.g., 20/merge => 20

        // Search for "TestApi.csproj" starting from the current directory
        var apiProjectFileName = "TestApi.csproj";

        // Start the project using dotnet run
        var apiProcessManager = new ApiProcessManager();
        await apiProcessManager.DotnetRun(apiProjectFileName);

        fuseDrillBaseAddres = "http://localhost:5184/";
        fuseDrillOpenApiUrl = "http://localhost:5184/swagger/v1/swagger.json";

        #if DEBUG
                githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
                pullReqestNumber = "20";
                branch = "feedToAiDiff";
                repoName = "FuseDrill";
                owner = "martasp";
        #endif

        await CliFlow(owner, repoName, branch, githubToken, fuseDrillBaseAddres, fuseDrillOpenApiUrl, fuseDrillTestAccountOAuthHeaderValue, smokeFlag, pullReqestNumber);
        await apiProcessManager.DisposeAsync();
    }
}
