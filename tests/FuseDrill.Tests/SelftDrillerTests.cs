using tests.Fuzzer;
namespace tests;

[CollectionDefinition("Sequential Tests", DisableParallelization = true)]
[Collection("Sequential Tests")]
public class SelfDrillerTests
{
    [Fact]
    public async Task DogFoodingTest()
    {
        var testApplication = new TestApplication<Program>();
        var client = testApplication.CreateClient();
        var apiUrl = client.BaseAddress.ToString();
        var openApiUrl = "/swagger/v1/swagger.json";

        var tester = new ApiFuzzerWithVerifier(new ApiFuzzer(client, client, openApiUrl));
        await tester.TestWholeApi();
    }

    [Fact]
    public async Task DogFoodingTestWithApplicationFactory()
    {
        var tester = new ApiFuzzerWithVerifier(new ApiFuzzer<Program>());
        await tester.TestWholeApi();
    }
}

