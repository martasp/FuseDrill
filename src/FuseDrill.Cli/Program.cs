using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Octokit;
using Fuzzer;
using tests.Fuzzer;


//Run printenv
//SELENIUM_JAR_PATH=/usr/share/java/selenium-server.jar
//CONDA=/usr/share/miniconda
//GITHUB_WORKSPACE=/home/runner/work/FuseDrill/FuseDrill
//JAVA_HOME_11_X64=/usr/lib/jvm/temurin-11-jdk-amd64
//GITHUB_PATH=/home/runner/work/_temp/_runner_file_commands/add_path_3b401634-32ff-47f0-ab1f-a77c309a9daf
//GITHUB_ACTION=__run_3
//JAVA_HOME=/usr/lib/jvm/temurin-11-jdk-amd64
//DOTNET_ROOT=/usr/share/dotnet
//GITHUB_RUN_NUMBER=4
//RUNNER_NAME=GitHub Actions 16
//GRADLE_HOME=/usr/share/gradle-8.11.1
//GITHUB_REPOSITORY_OWNER_ID=13511209
//ACTIONS_RUNNER_ACTION_ARCHIVE_CACHE=/opt/actionarchivecache
//XDG_CONFIG_HOME=/home/runner/.config
//DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
//ANT_HOME=/usr/share/ant
//JAVA_HOME_8_X64=/usr/lib/jvm/temurin-8-jdk-amd64
//GITHUB_TRIGGERING_ACTOR=martasp
//GITHUB_REF_TYPE=branch
//HOMEBREW_CLEANUP_PERIODIC_FULL_DAYS=3650
//ANDROID_NDK=/usr/local/lib/android/sdk/ndk/27.2.12479018
//BOOTSTRAP_HASKELL_NONINTERACTIVE=1
//***
//PIPX_BIN_DIR=/opt/pipx_bin
//STATS_TRP=true
//GITHUB_REPOSITORY_ID=899104492
//DEPLOYMENT_BASEPATH=/opt/runner
//GITHUB_ACTIONS=true
//STATS_VMD=true
//ANDROID_NDK_LATEST_HOME=/usr/local/lib/android/sdk/ndk/27.2.12479018
//SYSTEMD_EXEC_PID=478
//GITHUB_SHA=662c12dfe19c0e1b3a648b852b395f811bc7ca09
//GITHUB_WORKFLOW_REF=martasp/FuseDrill/.github/workflows/FuseDrillSnapshot.yml@refs/heads/main
//POWERSHELL_DISTRIBUTION_CHANNEL=GitHub-Actions-ubuntu22
//RUNNER_ENVIRONMENT=github-hosted
//STATS_EXTP=https://provjobdprod.z13.web.core.windows.net/settings/provjobdsettings-latest/provjobd.data
//DOTNET_MULTILEVEL_LOOKUP=0
//GITHUB_REF=refs/heads/main
//RUNNER_OS=Linux
//GITHUB_REF_PROTECTED=false
//HOME=/home/runner
//GITHUB_API_URL=https://api.github.com
//LANG=C.UTF-8
//RUNNER_TRACKING_ID=github_76ad1b46-b12e-41c8-901e-1f9de3cac37a
//RUNNER_ARCH=X64
//GOROOT_1_21_X64=/opt/hostedtoolcache/go/1.21.13/x64
//RUNNER_TEMP=/home/runner/work/_temp
//GITHUB_STATE=/home/runner/work/_temp/_runner_file_commands/save_state_3b401634-32ff-47f0-ab1f-a77c309a9daf
//STATS_PIP=false
//EDGEWEBDRIVER=/usr/local/share/edge_driver
//JAVA_HOME_21_X64=/usr/lib/jvm/temurin-21-jdk-amd64
//GITHUB_ENV=/home/runner/work/_temp/_runner_file_commands/set_env_3b401634-32ff-47f0-ab1f-a77c309a9daf
//GITHUB_EVENT_PATH=/home/runner/work/_temp/_github_workflow/event.json
//INVOCATION_ID=effd931a05514475bb0ccb1e60756fe6
//STATS_D=true
//GITHUB_EVENT_NAME=push
//GITHUB_RUN_ID=12184559543
//JAVA_HOME_17_X64=/usr/lib/jvm/temurin-17-jdk-amd64
//ANDROID_NDK_HOME=/usr/local/lib/android/sdk/ndk/27.2.12479018
//GITHUB_STEP_SUMMARY=/home/runner/work/_temp/_runner_file_commands/step_summary_3b401634-32ff-47f0-ab1f-a77c309a9daf
//HOMEBREW_NO_AUTO_UPDATE=1
//GITHUB_ACTOR=martasp
//NVM_DIR=/home/runner/.nvm
//SGX_AESM_ADDR=1
//GITHUB_RUN_ATTEMPT=1
//STATS_RDCL=true
//ANDROID_HOME=/usr/local/lib/android/sdk
//GITHUB_GRAPHQL_URL=https://api.github.com/graphql
//RUNNER_USER=runner
//ACCEPT_EULA=Y
//STATS_UE=true
//USER=runner
//GITHUB_SERVER_URL=https://github.com
//STATS_V3PS=true
//PIPX_HOME=/opt/pipx
//GECKOWEBDRIVER=/usr/local/share/gecko_driver
//STATS_EXT=true
//CHROMEWEBDRIVER=/usr/local/share/chromedriver-linux64
//SHLVL=1
//ANDROID_SDK_ROOT=/usr/local/lib/android/sdk
//VCPKG_INSTALLATION_ROOT=/usr/local/share/vcpkg
//GITHUB_ACTOR_ID=13511209
//RUNNER_TOOL_CACHE=/opt/hostedtoolcache
//ImageVersion=20241201.1.0
//DOTNET_NOLOGO=1
//GOROOT_1_23_X64=/opt/hostedtoolcache/go/1.23.3/x64
//GITHUB_WORKFLOW_SHA=662c12dfe19c0e1b3a648b852b395f811bc7ca09
//GITHUB_REF_NAME=main
//GITHUB_JOB=build
//XDG_RUNTIME_DIR=/run/user/1001
//AZURE_EXTENSION_DIR=/opt/az/azcliextensions
//PERFLOG_LOCATION_SETTING=RUNNER_PERFLOG
//STATS_VMFE=true
//GITHUB_REPOSITORY=martasp/FuseDrill
//ANDROID_NDK_ROOT=/usr/local/lib/android/sdk/ndk/27.2.12479018
//CHROME_BIN=/usr/bin/google-chrome
//GOROOT_1_22_X64=/opt/hostedtoolcache/go/1.22.9/x64
//GITHUB_RETENTION_DAYS=90
//JOURNAL_STREAM=8:18280
//RUNNER_WORKSPACE=/home/runner/work/FuseDrill
//LEIN_HOME=/usr/local/lib/lein
//LEIN_JAR=/usr/local/lib/lein/self-installs/leiningen-2.11.2-standalone.jar
//GITHUB_ACTION_REPOSITORY=
//PATH=/usr/share/dotnet:/snap/bin:/home/runner/.local/bin:/opt/pipx_bin:/home/runner/.cargo/bin:/home/runner/.config/composer/vendor/bin:/usr/local/.ghcup/bin:/home/runner/.dotnet/tools:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:/usr/games:/usr/local/games:/snap/bin
//RUNNER_PERFLOG=/home/runner/perflog
//GITHUB_BASE_REF=
//GHCUP_INSTALL_BASE_PREFIX=/usr/local
//CI=true
//SWIFT_PATH=/usr/share/swift/usr/bin
//ImageOS=ubuntu22
//STATS_D_D=true
//GITHUB_REPOSITORY_OWNER=martasp
//GITHUB_HEAD_REF=
//GITHUB_ACTION_REF=
//STATS_D_TC=true
//GITHUB_WORKFLOW=.NET
//DEBIAN_FRONTEND=noninteractive
//GITHUB_OUTPUT=/home/runner/work/_temp/_runner_file_commands/set_output_3b401634-32ff-47f0-ab1f-a77c309a9daf
//AGENT_TOOLSDIRECTORY=/opt/hostedtoolcache
//_=/usr/bin/printenv

// Get all GitHub Actions environment variables
var envars = Environment.GetEnvironmentVariables();

var owner = envars["GITHUB_REPOSITORY_OWNER"]?.ToString(); // e.g., "owner"
var repoName = envars["GITHUB_REPOSITORY"]?.ToString()?.Split('/')?[1]; // e.g., "repo-name"
var branch = envars["GITHUB_REF_NAME"]?.ToString(); // e.g., "refs/heads/branch-name"
var token = envars["GITHUB_TOKEN"]?.ToString();

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
var snapshotString = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });

var version = 0;
// Save snapshot to a local file
var fileName = $"api-snapshot-{version}.json";

// GitHub client setup
var github = new GitHubClient(new ProductHeaderValue("FuseDrill"));

// Authenticate GitHub client using a token (replace with your token)
var tokenAuth = new Credentials(token);
github.Credentials = tokenAuth;

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

