using System.Diagnostics;

public class ApiProcessManager : IAsyncDisposable
{
    private Process? _apiProcess;

    public static string? ProcessIdFile { get; private set; } = "lastProcessId.txt";

    private string FindProjectPath(string projectFileName)
    {
        // Get the current directory
        string currentDirectory = Directory.GetCurrentDirectory();

        // Number of levels to go up
        int levelsToGoUp = 4;

        // Get the desired parent directory or null if it doesn't exist
        var directory = Enumerable.Range(0, levelsToGoUp)
            .Aggregate(currentDirectory, (current, _) => Directory.GetParent(current)?.FullName)
            ?? "No parent found";

        // Search for the project file in all projects of the solution
        var projectPaths = Directory.EnumerateFiles(directory, projectFileName, SearchOption.AllDirectories)
            .ToList();

        return projectPaths.FirstOrDefault(); // Return the first matching project path
    }

    public async Task DotnetRun(string apiProjectFileName)
    {
        KillPreviousProcessIfExists();

        var projectPath = FindProjectPath(apiProjectFileName);

        if (projectPath == null)
        {
            throw new FileNotFoundException($"Could not find '{apiProjectFileName}' in any parent directory.");
        }

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _apiProcess = new Process
        {
            StartInfo = processStartInfo
        };

        _apiProcess.OutputDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
                Console.WriteLine(args.Data);
        };

        _apiProcess.ErrorDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
                Console.WriteLine($"ERROR: {args.Data}");
        };

        _apiProcess.Start();
        _apiProcess.BeginOutputReadLine();
        _apiProcess.BeginErrorReadLine();

        // Write the process ID to a temp file
        File.WriteAllText(ProcessIdFile, _apiProcess.Id.ToString());

        // Wait a few seconds to allow the process to start before proceeding
        await Task.Delay(5000);

    }

    public static void KillPreviousProcessIfExists()
    {
        // If the file doesn't exist, there is no previous process to kill
        if (!File.Exists(ProcessIdFile))
            return;

        // Attempt to read and parse the process ID from the file
        var processIdText = File.ReadAllText(ProcessIdFile);
        if (!int.TryParse(processIdText, out int processId))
            return;

        // Try to get and kill the process if it's running
        try
        {
            var process = Process.GetProcessById(processId);
            if (process.HasExited)
                return;

            process.Kill();
            process.WaitForExit();
        }
        catch (ArgumentException)
        {
            // Process with the given ID is not running or has already exited
            // Ignore this exception as it’s expected in this case
        }
        catch (InvalidOperationException)
        {
            // Process has already exited between check and kill call
            // Ignore this exception as well
        }
        finally
        {
            // Clean up the process ID file to prevent future errors
            File.Delete(ProcessIdFile);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_apiProcess != null && !_apiProcess.HasExited)
        {
            // Gracefully shut down the API process
            _apiProcess.Kill();
            await _apiProcess.WaitForExitAsync(); // Wait for the process to exit
        }
    }
}
