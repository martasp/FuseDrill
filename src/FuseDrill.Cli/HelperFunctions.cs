using FuseDrill.Core;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Octokit;
using System.ClientModel.Primitives;
using System.Text.Json;

public static class HelperFunctions
{
    public static async Task<bool> CliFlow(string? owner, string? repoName, string? branch, string? githubToken, string? fuseDrillBaseAddres, string? fuseDrillOpenApiUrl, string? fuseDrillTestAccountOAuthHeaderValue, bool smokeFlag, string? pullRequestNumber, string? geminiToken)
    {
        // Fuzz testing the API
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(fuseDrillBaseAddres),
        };

        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", fuseDrillTestAccountOAuthHeaderValue);

        var tester = new ApiFuzzer(httpClient, fuseDrillOpenApiUrl);
        var snapshot = await tester.TestWholeApi();
        var newSnapshotString = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true, Converters = { new DateTimeScrubbingConverter(), new GuidScrubbingConverter(), new DateTimeOffsetScrubbingConverter() }, });

        if (smokeFlag)
        {
            Console.WriteLine(newSnapshotString);
        }

        if (string.IsNullOrEmpty(newSnapshotString))
        {
            Console.WriteLine("API snapshot is empty.");
            return false;
        }

        // Save snapshot to a local file
        var filePath = $"api-snapshot.json";

        // GitHub client setup
        var githubClient = new GitHubClient(new ProductHeaderValue("FuseDrill"));

        // Authenticate GitHub client using a githubToken (replace with your githubToken)
        var tokenAuth = new Credentials(githubToken);
        githubClient.Credentials = tokenAuth;

        if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(repoName))
        {
            Console.WriteLine("Repository owner or name not found in environment variables.");
            return false;
        }

        if (string.IsNullOrEmpty(branch))
        {
            Console.WriteLine("Branch name not found in environment variables.");
            return false;
        }

        // Read the branch reference
        var branchRef = $"refs/heads/{branch}";

        var existingSnapshotString = await GetExistingSnapshotAsync(owner, repoName, branch, filePath, githubClient);
        await SaveSnapshotAsync(owner, repoName, branch, newSnapshotString, filePath, githubClient, branchRef);

        if (existingSnapshotString == "")
        {
            Console.WriteLine("No existing snapshot found.");
            Console.WriteLine("Stopping comparison.");
            return false;
        }

        if (!int.TryParse(pullRequestNumber, out var pullRequestNumberParsed))
        {
            Console.WriteLine("Pull request number does not exists");
            return false;
        }

        if (string.IsNullOrEmpty(geminiToken))
        {
            Console.WriteLine("Gemini token is not provided, continuing without AI summarization");
            return false;
        }

        string llmResponse = await CompareFuzzingsWithLLM(newSnapshotString, existingSnapshotString);

        await PostCommentToPullRequestAsync(owner, repoName, pullRequestNumberParsed, llmResponse, githubClient);

        Console.WriteLine(llmResponse);
        return true;
    }


    private static async Task PostCommentToPullRequestAsync(string owner, string repoName, int pullRequestNumber, string comment, GitHubClient githubClient)
    {
        Console.WriteLine($"Creating comment at PR:{pullRequestNumber}");
        await githubClient.Issue.Comment.Create(owner, repoName, pullRequestNumber, comment);
    }


    public static async Task<string> AnalyzeFuzzingDiffWithLLM(Kernel kernel, string fuzzingOutputDiff)
    {
        var prompt =
    """
Here’s an improved version of your prompt that incorporates markdown checkboxes for actionable items in the output:

---

### Prompt for API Contract Reviews
**Context:**  
You are an expert in reviewing API contracts and changes for adherence to best practices, compatibility, and potential breaking changes. The API contracts use JSON structures, and I provide you with the differences between the previous version and the current version of the contract. Your task is to:  
1. Identify and explain the nature of changes.  
2. Analyze potential impacts on compatibility and clients using the API.  
3. Suggest improvements or highlight issues in the changes.  

**Example Contract Difference:**  
```json
"Request": {
  "Id": 4,
- "Breed": "RandomString275",
- "Name": "RandomString157",
+ "Type": "RandomString275",
+ "FullName": "RandomString157",
+ "Surname": "RandomString601",
+ "Age": 4,
  "PetType": 1
}
```

**Your task:**  
1. Provide a detailed summary of the changes in the API contract.  
2. Evaluate if the changes break backward compatibility.  
3. Suggest any improvements or identify issues based on best practices for API design.  
4. Output actionable feedback using markdown, with a focus on checkboxes for clear action items.
5. Look into other issues in the diff, meybe status codes is changed, maybe request fails with 500 and others.

**Deliver the response as:**  
- A concise but detailed summary of the analysis.  
- A one property dif at a time.
- A markdown-formatted list of backward compatibility concerns.  
- A markdown-formatted checklist of actionable recommendations for improving the contract.
- Always use checklist - [ ] for every action point.
- Produce valid markdown.
---

### Expected Example of the LLM's Output
**Summary of Changes:**  
- The field `Breed` has been replaced with `Type`.  
- The field `Name` has been replaced with `FullName`, and new fields `Surname` and `Age` have been added.  
- No changes have been made to the `PetType` field.  

**Backward Compatibility Concerns:**  
- The removal of `Breed` and `Name` fields is a breaking change for clients relying on these fields.  
- Clients using the API must update their integrations to accommodate `Type` and `FullName`.  

**Actionable Recommendations:**  
- [ ] **Deprecate fields instead of immediate removal:** Keep `Breed` and `Name` temporarily with a warning about their future removal.  
- [ ] **Provide clear migration documentation:** Explain how clients should map `Breed` to `Type` and `Name` to `FullName`.  
- [ ] **Ensure default values for new fields:** Add sensible defaults for `Surname` and `Age` to minimize client-side errors.  
- [ ] **Test the changes thoroughly:** Validate the new contract with sample clients to identify potential issues early.  

---

This version ensures actionable items are clearly outlined for easy tracking and implementation.

Heres is Contract Difference :
----
{{$fuzzingOutputDiff}}
----
""";
        var code = kernel.CreateFunctionFromPrompt(prompt, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 20000 });

        var markdownResponse = await kernel.InvokeAsync(code, new()
        {
            ["fuzzingOutputDiff"] = fuzzingOutputDiff,
        });

        return markdownResponse.ToString();
    }

    public static async Task<string> GetExistingSnapshotAsync(
        string owner,
        string repoName,
        string branch,
        string filePath,
        GitHubClient githubClient)
    {
        // GitHub client setup

        try
        {
            // Get the file contents from the repository by branch reference
            var branchRef = $"refs/heads/{branch}";
            var fileContents = await githubClient.Repository.Content.GetAllContentsByRef(owner, repoName, filePath, branchRef);

            // Return the content of the file
            return fileContents[0].Content; // Assuming the file exists and is not a directory
        }
        catch (NotFoundException)
        {
            Console.WriteLine($"The file '{filePath}' does not exist in branch '{branch}'.");
            return string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while fetching the snapshot: {ex.Message}");
            return string.Empty;
        }
    }

    public static async Task SaveSnapshotAsync(string? owner, string? repoName, string? branch, string newSnapshotString, string filePath, GitHubClient githubClient, string branchRef)
    {

        // Get the reference for the branch
        var reference = await githubClient.Git.Reference.Get(owner, repoName, branchRef);

        // Create a blob for the snapshot file
        var blob = new NewBlob
        {
            Content = newSnapshotString,
            Encoding = EncodingType.Utf8
        };

        var blobResult = await githubClient.Git.Blob.Create(owner, repoName, blob);

        // Create a tree with the new blob
        var newTree = new NewTree
        {
            BaseTree = reference.Object.Sha
        };

        newTree.Tree.Add(new NewTreeItem
        {
            Path = filePath,
            Mode = "100644",
            Type = TreeType.Blob,
            Sha = blobResult.Sha
        });

        var treeResult = await githubClient.Git.Tree.Create(owner, repoName, newTree);

        // Create a new commit
        var newCommit = new NewCommit("Add API snapshot JSON", treeResult.Sha, reference.Object.Sha);

        var commitResult = await githubClient.Git.Commit.Create(owner, repoName, newCommit);

        // Update the reference to point to the new commit
        await githubClient.Git.Reference.Update(owner, repoName, branchRef, new ReferenceUpdate(commitResult.Sha));

        Console.WriteLine($"Snapshot committed to branch {branch} in repository {owner}/{repoName}");
    }

    public static async Task<string> CompareFuzzingsWithLLM(string newSnapshotString, string existingSnapshotString)
    {
        //use difplex string comparison 
        var actualDiff = SimpleDiffer.GenerateDiff(existingSnapshotString, newSnapshotString);

        //use semantic kernel 
        var kernel = Kernel.CreateBuilder()
        .AddLMStudioChatCompletionGemini()
        .Build();


        var llmResponse = await AnalyzeFuzzingDiffWithLLM(kernel, actualDiff);
        return llmResponse;
    }

}
