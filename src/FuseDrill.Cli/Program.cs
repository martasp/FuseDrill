using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Octokit;
using Fuzzer;
using tests.Fuzzer;

// Get all GitHub Actions environment variables
var envars = Environment.GetEnvironmentVariables();

// Fetch specific variables
var pullRequest = envars["GITHUB_PULL_REQUEST"]?.ToString();
var branch = envars["GITHUB_REF_NAME"]?.ToString(); // e.g., "refs/heads/branch-name"

if (string.IsNullOrEmpty(branch))
{
    Console.WriteLine("Branch name not found in environment variables.");
    return;
}

// API details
var baseAddress = "https://api.apis.guru/v2";
var openApiUrl = "https://api.apis.guru/v2/openapi.yaml";

// Fuzz testing the API
var httpClient = new HttpClient
{
    BaseAddress = new Uri(baseAddress)
};

var tester = new ApiFuzzer(httpClient, openApiUrl);
var snapshot = await tester.TestWholeApi();
var snapshotString = JsonSerializer.Serialize(snapshot);

// Save snapshot to a local file
var fileName = $"api-snapshot-{DateTime.Now.ToFileTimeUtc()}.json";

// GitHub client setup
var github = new GitHubClient(new ProductHeaderValue("FuseDrill"));

// Authenticate GitHub client using a token (replace with your token)
var tokenAuth = new Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
github.Credentials = tokenAuth;

// Repository details
var owner = envars["GITHUB_REPOSITORY_OWNER"]?.ToString(); // e.g., "owner"
var repoName = envars["GITHUB_REPOSITORY"]?.ToString()?.Split('/')?[1]; // e.g., "repo-name"

if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(repoName))
{
    Console.WriteLine("Repository owner or name not found in environment variables.");
    return;
}

// Read the branch reference
var branchRef = $"heads/{branch}";

// Get the reference for the branch
var reference = await github.Git.Reference.Get(owner, repoName, branchRef);

// Create a blob for the snapshot file
var blob = new NewBlob
{
    Content = snapshotString,
    Encoding = EncodingType.Utf8
};

var blobResult = await github.Git.Blob.Create(owner, repoName, blob);

// Create a tree with the new blob
var newTree = new NewTree
{
    BaseTree = reference.Object.Sha
};

newTree.Tree.Add(new NewTreeItem
{
    Path = fileName,
    Mode = "100644",
    Type = TreeType.Blob,
    Sha = blobResult.Sha
});

var treeResult = await github.Git.Tree.Create(owner, repoName, newTree);

// Create a new commit
var newCommit = new NewCommit("Add API snapshot JSON", treeResult.Sha, reference.Object.Sha);

var commitResult = await github.Git.Commit.Create(owner, repoName, newCommit);

// Update the reference to point to the new commit
await github.Git.Reference.Update(owner, repoName, branchRef, new ReferenceUpdate(commitResult.Sha));

Console.WriteLine($"Snapshot committed to branch {branch} in repository {owner}/{repoName}");

