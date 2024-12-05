using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace Tests;
public class TodoEndpointsV1Tests : IClassFixture<TestApplication<Program>>
{
    private readonly TestApplication<Program> _factory;
    private readonly HttpClient _httpClient;

    public TodoEndpointsV1Tests(TestApplication<Program> factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();

    }

    public async Task PostTodoWithValidationProblems()
    {
        var response = await _httpClient.GetAsync("/WeatherForecast");

        var resp = response.Content.ReadAsStringAsync();

        var problemResult = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

    }
}
