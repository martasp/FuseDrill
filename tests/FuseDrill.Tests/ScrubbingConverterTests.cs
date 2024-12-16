using System.Text.Json;
namespace tests;

public class ScrubbingConverterTests
{
    [Fact]
    public async Task DateAndGuidAndDateTimeOffsetValuesShouldBeScrubbed()
    {
        var testData = new
        {
            Id = new { id = Guid.NewGuid(), date = DateTimeOffset.Now }, 
            Name = "In-Memory Test Data",
            Date = DateTime.Now,  
            Tags = new[] { "example", "snapshot", "memory" }
        };

        var json = JsonSerializer.Serialize(testData, new JsonSerializerOptions { WriteIndented = true, Converters = { new DateTimeScrubbingConverter(), new GuidScrubbingConverter(), new DateTimeOffsetScrubbingConverter() }, });

        await Verify(json);
    }
}

