using Microsoft.VisualStudio.TestPlatform.TestHost;

public class MainTests
{
    [Fact(Skip = "Test Uses LLM, only good for manual debugging and tweaking prompt")]
    //[Fact]
    public async Task CompareFuzzingsWithLLMTest()
    {

        string oldText =
                    """
                    {
                      "Name": "John",
                      "Surname": "Doe",
                      "Age": 20,
                      "Grade": "A"
                    }
                    """;

        string newText =
                    """
                    {
                      "Name": "John",
                      "Surname": "Doe",
                      "Grade": "B"
                    }
                    """;

        var resut = await HelperFunctions.CompareFuzzingsWithLLM(oldText, newText);
    }
}
