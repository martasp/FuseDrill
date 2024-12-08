//using Coverlet.Console.Logging;
//using Coverlet.Core.Abstractions;
//using Coverlet.Core.Helpers;
//using Coverlet.Core.Symbols;
//using Coverlet.Core;
//using Microsoft.Extensions.DependencyInjection;


////Not working in process is more complicated.
//public class Cover()
//{
//    //Programatic coverage is hard now : https://github.com/coverlet-coverage/coverlet/blob/7c8c6fae2715308a0116e2b40221f06cbf07e7bd/src/coverlet.console/Program.cs#L199
//    private Coverage InstrumentAssembly(string assemblyPath)
//    {

//        string sourceMappingFile = null;
//        IServiceCollection serviceCollection = new ServiceCollection();
//        serviceCollection.AddTransient<IRetryHelper, RetryHelper>();
//        serviceCollection.AddTransient<IProcessExitHandler, ProcessExitHandler>();
//        serviceCollection.AddTransient<IFileSystem, FileSystem>();
//        serviceCollection.AddTransient<Logger, ConsoleLogger>();
//        // We need to keep singleton/static semantics
//        serviceCollection.AddSingleton<IInstrumentationHelper, InstrumentationHelper>();
//        serviceCollection.AddSingleton<ISourceRootTranslator, SourceRootTranslator>(provider => new SourceRootTranslator(sourceMappingFile, provider.GetRequiredService<Coverlet.Core.Abstractions.Logger>(), provider.GetRequiredService<IFileSystem>()));
//        serviceCollection.AddSingleton<ICecilSymbolHelper, CecilSymbolHelper>();

//        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
//        var logger = serviceProvider.GetService<Logger>();
//        IFileSystem fileSystem = serviceProvider.GetService<IFileSystem>();

//        // Hardcoded parameters for in-memory coverage collection
//        var parameters = new CoverageParameters
//        {
//            IncludeFilters = Array.Empty<string>(),  // Include everything
//            //IncludeFilters = new[] { "[*]*" },  // Include everything
//            //IncludeDirectories = new[] { @"D:\main\Pocs\newpocs\TestApi\bin\Debug\net8.0" },  // Directory containing your assemblies
//            IncludeDirectories = { },  // Directory containing your assemblies
//            ExcludeFilters = new[] { "[NUnit.*]*", "[xunit.*]*", "[coverlet.*]*", "[tests*]*" },  // Exclude testing-related assemblies
//            //ExcludeFilters = { },  // Exclude testing-related assemblies
//            ExcludedSourceFiles = Array.Empty<string>(),  // Leave empty to include all files
//            ExcludeAttributes = Array.Empty<string>(),  // No specific attributes to exclude
//            IncludeTestAssembly = false,  // Ensure the test assembly is included for coverage
//            SingleHit = false,  // Allow multiple hits on a single line
//            MergeWith = null,  // Not merging with other reports
//            UseSourceLink = false,  // SourceLink disabled for this configuration
//            SkipAutoProps = false,  // Include auto-properties for coverage
//            DoesNotReturnAttributes = Array.Empty<string>(),  // Default setting
//            ExcludeAssembliesWithoutSources = null,

//        };

//        // Assuming serviceProvider is defined elsewhere in the class
//        ISourceRootTranslator sourceRootTranslator = serviceProvider.GetRequiredService<ISourceRootTranslator>();

//        // Create Coverage instance with hardcoded parameters
//        Coverage coverage = new(assemblyPath,
//                                         parameters,
//                                         logger,
//                                         serviceProvider.GetRequiredService<IInstrumentationHelper>(),
//                                         fileSystem,
//                                         sourceRootTranslator,
//                                         serviceProvider.GetRequiredService<ICecilSymbolHelper>());
//        // Instrument the assembly
//        coverage.PrepareModules();

//        return coverage;
//    }


//    private double CalculateCoveragePercentage(CoverageResult result)
//    {
//        var summary = new CoverageSummary();

//        CoverageDetails linePercentCalculation = summary.CalculateLineCoverage(result.Modules);
//        CoverageDetails branchPercentCalculation = summary.CalculateBranchCoverage(result.Modules);
//        CoverageDetails methodPercentCalculation = summary.CalculateMethodCoverage(result.Modules);

//        double totalLinePercent = linePercentCalculation.Percent;
//        double totalBranchPercent = branchPercentCalculation.Percent;
//        double totalMethodPercent = methodPercentCalculation.Percent;

//        double averageLinePercent = linePercentCalculation.AverageModulePercent;
//        double averageBranchPercent = branchPercentCalculation.AverageModulePercent;
//        double averageMethodPercent = methodPercentCalculation.AverageModulePercent;

//        return averageLinePercent;
//    }

//}