//using NSwag;
//using NSwag.CodeGeneration.CSharp;
//using System.Reflection;
//using FsCheck;
//using FsCheck.Xunit;
//using GeneratedClientMyNamespace;
//using OneOf;
//using System.ComponentModel.DataAnnotations;
//using ListPermutationExtension;
//using Coverlet.Core;
////using Newtonsoft.Json;
//using Coverlet.Core.Abstractions;
//using Microsoft.Extensions.DependencyInjection;
//using Coverlet.Core.Helpers;
//using Coverlet.Core.Symbols;
//using System.Diagnostics;
//using Coverlet.Console.Logging;
//using System.Collections;
//using System.Text.Json.Serialization;
//using System.Text.Json;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.Extensions.Options;
//using Argon;
//namespace Tests;
//public class DrillerTests
//{
//    public TestApplication _factory;
//    public HttpClient _httpClient;

//    private static readonly object _fileLock = new object();


//    public DrillerTests()
//    {
//        _factory = new TestApplication();
//        _httpClient = _factory.CreateClient();
//    }

//    [Fact]
//    public void NonsenseQuery_ShouldNotThrowForRandomInput()
//    {
//        // Use FsCheck to generate random arrays and test the NonsenseQuery method
//        Prop.ForAll<int?[]?>(a =>
//        {
//            var exampleClass = new WebApiSample.Controllers.ExampleClass();


//            a = a.Length == 3 ? null : a;
//            // Act
//            var result = exampleClass.NonsenseQuery(a);

//            // Assert
//            if (a != null)
//            {
//                Assert.NotNull(result);
//            }
//            else
//            {
//                Assert.Null(result);
//            }
//        }).QuickCheckThrowOnFailure();
//    }



//    public async Task GenerateTestSuiteWithForAll() //PorpForAll not working as expected
//    {
//        // Define a fixed seed for reproducibility
//        var seed = 42;

//        var instance = await GetSwaggerClientInstance();
//        var suite = new ArbitraryTestSuite().CreateList(new DataModel(instance));

//        // Snapshot testing setup

//        var ok = new List<TestSuite>();

//        Prop.ForAll<TestSuite>(async (TestSuite testSuite) =>
//        {
//            // Start capturing code coverage here (implementation-dependent)
//            foreach (var apiCal in testSuite.ApiCalls.ToList())
//            {
//                var api = await DoAttempt3(instance, apiCal);
//                apiCal.Response = api.Response;
//            }

//            // Here we would compute and set testSuite.TestCoveragePercentage based on code coverage

//            ok.Add(testSuite);
//        }).QuickCheckThrowOnFailure(); // Add seed for reproducibility


//    }

//    //You can build with from console : 
//    //    coverlet D:\main\Pocs\newpocs\tests\bin\Debug\net8.0\tests.dll --target "dotnet" --targetargs "test D:\main\Pocs\newpocs\tests\tests.csproj --no-build"
//    //Coverlet in process does not work, because never hits this class: ModuleTrackerTemplate
//    [Fact]
//    public async Task GenerateTestSuite()
//    {
//        var settings = new VerifySettings();
//        settings.UseStrictJson();
//        settings.DontIgnoreEmptyCollections();

//        var testSuites = new List<TestSuite>();
//        var instance = await GetSwaggerClientInstance();
//        var apiClientAsData = new DataModel(instance);
//        var testSuiteGen = new ArbitraryTestSuite().CreateList(apiClientAsData);

//        // Automatically get the path of the currently executing assembly
//        //string assemblyPath = @"D:\main\Pocs\newpocs\tests\bin\Debug\net8.0\tests.dll";

//        foreach (var testSuite in testSuiteGen)
//        {
//            // Step 1: Instrument the assembly to collect coverage for the current test suite
//            //var coverage = InstrumentAssembly(assemblyPath);

//            //Step 2: Run your tests (simulate custom test execution)
//            testSuite.ApiCalls = testSuite.ApiCalls.OrderBy(item => item.Order).ToList();
//            foreach (var apiCal in testSuite.ApiCalls)
//            {
//                var api = await DoAttempt3(instance, apiCal);
//                apiCal.Response = ResolveResponse(api);
//                // Pass the unique prefix to Verifier
//            }
//            RecreateFixture();
//            instance = await RecreateSwaggerClientInstance();
//            //var result = coverage.GetCoverageResult();

//            // Step 3: Calculate and set the test coverage percentage for the current test suite
//            //testSuite.TestCoveragePercentage = CalculateCoveragePercentage(result);

//            // Add the test suite results to the output list
//            testSuites.Add(testSuite);

//        }

//        await Verifier.Verify(testSuites, settings);

//        static dynamic ResolveResponse(ApiCall api)
//        {
//            var exception = api.Response as ApiException;
//            var simplifiedException = new { exception?.StatusCode, exception?.GetType().Name };


//            var res = api.Response switch
//            {
//                ApiException => simplifiedException,
//                //IList list => new {listCount = list.Count},
//                object => api.Response,
//                _ => throw new Exception("cant parse")
//            };

//            return res;
//        }
//    }

//    private void RecreateFixture()
//    {
//        // Dispose of the existing factory and HttpClient
//        _httpClient.Dispose();
//        _factory.Dispose();

//        // Create a new instance of the TestApplication
//        _factory = new TestApplication();
//        _httpClient = _factory.CreateClient();
//    }


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

//    public async Task<Object> GetSwaggerClientInstance()
//    {
//        var response = await _httpClient.GetAsync("/swagger/v1/swagger.json");
//        var resp = response.Data.ReadAsStringAsync();

//        var document = await OpenApiDocument.FromJsonAsync(resp.Result);

//        var settings = new CSharpClientGeneratorSettings
//        {
//            ClassName = "GeneratedClient",
//            ExposeJsonSerializerSettings = true,

//            CSharpGeneratorSettings =
//            {
//                Namespace = "GeneratedClientMyNamespace"
//            }
//        };

//        var generator = new CSharpClientGenerator(document, settings);
//        var code = generator.GenerateFile();
//        using (StreamWriter writer = new StreamWriter("D:\\main\\Pocs\\newpocs\\tests\\GeneratedClientMyNamespace\\GeneratedClient.cs", false))
//        {
//            await writer.WriteAsync(code);
//        }

//        var myClassInstance = new GeneratedClientMyNamespace.GeneratedClient("http://localhost/", _httpClient);
//        return myClassInstance;
//    }

//    public async Task<Object> RecreateSwaggerClientInstance()
//    {
//        var myClassInstance = new GeneratedClientMyNamespace.GeneratedClient("http://localhost/", _httpClient);
//        return myClassInstance;
//    }

//    public static dynamic GenerateRandomValue(Type runtimeType)
//    {
//        return runtimeType switch
//        {
//            Type t when t == typeof(int) => Gen.Choose(int.MinValue, int.MaxValue),
//            Type t when t == typeof(string) => CustomStringGenerator(1, 10).ToString(),
//            Type t when t == typeof(bool) => Gen.Elements(true, false),
//            Type t when t == typeof(double) => Gen.Choose(0, 1),
//            _ => throw new InvalidOperationException("Unsupported type: " + runtimeType.Name),
//        };
//    }

//    public static Gen<string> CustomStringGenerator(int minLength, int maxLength)
//    {
//        var characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"; // Define the characters you want in the string
//        return Gen.Choose(minLength, maxLength)
//            .Select(length =>
//            {
//                var random = new System.Random();
//                return new string(Enumerable.Repeat(characters, length)
//                    .Select(s => s[random.Next(s.Length)]).ToArray());
//            });
//    }

//    public static async Task<ApiCall> DoAttempt2(object instance, ApiCall api) // object evenatully can be discriminated union, response will be object same applies.
//    {
//        async Task<bool> PetsGETAsync(ApiCall api)
//        {
//            await ((GeneratedClient)instance).PetsGETAsync((int)api.Request);
//            return true;
//        }

//        try
//        {
//            var res = api.methodName switch
//            {
//                MethodName.PetsPOSTAsync => await ((GeneratedClient)instance).PetsPOSTAsync((Pet)api.Request),
//                MethodName.PetsPUTAsync => await ((GeneratedClient)instance).PetsPUTAsync((Pet)api.Request),
//                MethodName.PetsGETAsync => await PetsGETAsync(api),
//                MethodName.PetsAllAsync => OneOf<ICollection<Pet>, Pet, bool, ApiException>.FromT0(await ((GeneratedClient)instance).PetsAllAsync()),
//                _ => throw new Exception("MethodNameNotSpecified"),
//            };

//            api.Response = res;

//        }
//        catch (ApiException ex)
//        {
//            api.Response = ex;
//        }

//        return api;

//    }

//    public static async Task<ApiCall> DoAttempt3(object instance, ApiCall api) // object evenatully can be discriminated union, response will be object same applies.
//    {
//        Debug.Assert(api?.methodName is not null, "Method name should be always filled.");

//        try
//        {
//            var method = ReflectionHelper.GetPublicEndpointMethodByName(instance.GetType(), api.methodName.GetDisplayName());
//            Assert.True(method.GetParameters().Length == 1 || method.GetParameters().Length == 0, "Currently we support void and one parameter in endpoint");

//            var parameter = ReflectionHelper.GetPublicEndpointMethodParameterByName(instance.GetType(), api.methodName.GetDisplayName());

//            var input = new object[] { };
//            if (parameter != null)
//            {
//                var request = Convert.ChangeType(((dynamic)api.Request), parameter.ParameterType);
//                input = new object[] { request };
//            }
//            else
//            {
//                input = null;
//            }

//            // Dynamically invoke the method on the instance
//            var task = (Task)method.Invoke(instance, input);
//            await task;

//            // Retrieve the result from the Task
//            var resultProperty = task.GetType().GetProperty("Result");
//            var result = resultProperty?.GetValue(task);

//            // Convert.ChangeType(result, api.Response.GetType());
//            api.Response = result;

//        }
//        catch (ApiException ex)
//        {
//            api.Response = ex;
//        }

//        return api;
//    }

//}


//public class ApiCall2
//{
//    public MethodName methodName { get; set; }
//    public int Order { get; set; } // From 1 to MethodName count;
//    public object Request { get; set; }
//    public object Response { get; set; }
//    public Type RequestType { get; set; }
//    public Type ResponseType { get; set; }
//}

//public class DataModel
//{
//    public List<Method> Methods { get; set; }

//    public DataModel(Object ClientInstance)
//    {
//        var methods = ReflectionHelper.GetPublicEndpointMethods(ClientInstance.GetType());
//        Methods = methods.Select(item => new Method
//        {
//            MethodName = item.Name,
//            MethodParameterTypeName = item?.GetParameters()?.FirstOrDefault()?.ParameterType?.Name ?? "None",
//            MethodParameterType = item?.GetParameters()?.FirstOrDefault()?.ParameterType ?? OneOf<Type, VoidEmptyType>.FromT1(new VoidEmptyType()),
//        }).ToList();
//    }

//    public string CreateRecords()
//    {

//        //public record PetsPOSTAsync_Pet(Pet Pet, MethodName MethodName = MethodName.PetsPOSTAsync);
//        //public record PetsPOSTAsync_Int(int id, MethodName MethodName = MethodName.PetsPOSTAsync);
//        //public record PetsPUTAsync(Pet Pet, MethodName MethodName = MethodName.PetsPUTAsync);
//        //public record PetsGETAsync(int id, MethodName MethodName = MethodName.PetsGETAsync);
//        //public record PetsAllAsync(int id, MethodName MethodName = MethodName.PetsAllAsync);
//        var lines = Methods.Select(item => @$"public record {item.MethodName}({item.MethodParameterTypeName} id, MethodName MethodName = MethodName.{item.MethodName});");
//        return string.Join(Environment.NewLine, lines);
//    }
//}

//public class Method
//{
//    public string MethodName { get; set; }
//    public string MethodParameterTypeName { get; set; }
//    public OneOf<Type, VoidEmptyType> MethodParameterType { get; set; }

//    //public OneOf<Type,VoidEmptyType> MethodParameterType => MethodParameterTypeName.ToType();
//}

//public class ApiCall3
//{
//    public MethodName methodName => (MethodName)Enum.Parse(typeof(MethodName), Request.Value.GetType().Name.Split("_")[0]);
//    public MethodName methodName1 => (MethodName)Request.Value.GetType().GetProperty("MethodName").GetValue(Request.Value, null);
//    public int Order { get; set; } // From 1 to MethodName count;
//    public OneOf<PetsPOSTAsync_Pet, PetsPOSTAsync_Int> Request { get; set; }
//    public object Response { get; set; }
//}

//public record PetsPOSTAsync_Pet(Pet Pet, MethodName MethodName = MethodName.PetsPOSTAsync);
//public record PetsPOSTAsync_Int(int id, MethodName MethodName = MethodName.PetsPOSTAsync);
//public record PetsPUTAsync(Pet Pet, MethodName MethodName = MethodName.PetsPUTAsync);
//public record PetsGETAsync(int id, MethodName MethodName = MethodName.PetsGETAsync);
//public record PetsAllAsync(int id, MethodName MethodName = MethodName.PetsAllAsync);

//public class ApiCall
//{
//    public MethodName methodName { get; set; }
//    public int Order { get; set; } // From 1 to MethodName count;
//    public object Request { get; set; }

//    //public OneOf<int, Pet> Request { get; set; }
//    //public OneOf<ICollection<Pet>, Pet, bool, ApiException> Response { get; set; }
//    //public OneOf<List<Object>,object> Response { get; set; } 
//    public object Response { get; set; }
//}

//public class Tests
//{
//    public List<TestSuite> TestSuites { get; set; }
//}


//public class TestSuite
//{
//    public List<ApiCall> ApiCalls { get; set; } = new List<ApiCall>();
//    public double TestCoveragePercentage { get; set; } = 0;
//}

//public enum MethodName
//{
//    None,
//    GetWeatherForecastAsync,
//    PetsPOSTAsync,
//    PetsPUTAsync,
//    PetsGETAsync,
//    PetsAllAsync,
//}

//public static class OneOfGenerators
//{
//    // Custom generator for OneOf<int, Pet>
//    public static Arbitrary<OneOf<int, GeneratedClientMyNamespace.Pet>> OneOfIntPet()
//    {
//        // Generator for the int case
//        var intGen = Arb.Generate<int>();

//        // Generator for the Pet case
//        var petGen = from name in Arb.Generate<string>()
//                     from breed in Arb.Generate<string>()
//                     from id in Arb.Generate<int>()
//                     from PetType in Arb.Generate<PetType>()
//                     select new GeneratedClientMyNamespace.Pet
//                     {
//                         Name = name,
//                         Breed = breed,
//                         Id = id,
//                         PetType = PetType,
//                     };

//        // Combine the generators for OneOf<int, Pet>
//        var oneOfGen = Gen.OneOf(
//            intGen.Select(OneOf<int, GeneratedClientMyNamespace.Pet>.FromT0),
//            petGen.Select(OneOf<int, GeneratedClientMyNamespace.Pet>.FromT1)
//        );

//        // Return the combined generator as an Arbitrary instance
//        return oneOfGen.ToArbitrary();
//    }

//    // Register the custom generator
//    public static Arbitrary<OneOf<int, GeneratedClientMyNamespace.Pet>> Generator()
//    {
//        return OneOfIntPet();
//    }
//}


//// Arbitrary Generators
//public class ArbitraryPet : Arbitrary<Pet>
//{
//    public override Gen<Pet> Generator =>
//        from id in Gen.Choose(1, 100)
//        from breed in Gen.Elements("Bulldog", "Beagle", "Poodle", "Labrador")
//        from name in Arb.Generate<string>().Where(n => !string.IsNullOrEmpty(n)) // Ensure name is not empty
//        from petType in Gen.Elements(PetType._1, PetType._0)
//        select new Pet { Id = id, Breed = breed, Name = name, PetType = petType };
//}

//public class ArbitraryApiCall : Arbitrary<ApiCall>
//{
//    public override Gen<ApiCall> Generator =>
//        from methodName in Gen.Elements(MethodName.PetsPOSTAsync, MethodName.PetsPUTAsync, MethodName.PetsGETAsync, MethodName.PetsAllAsync)
//        from order in Gen.Elements(1, 2, 3, 4) // The order is from 1 to the count of methods
//        select new ApiCall
//        {
//            methodName = methodName,
//            Order = order,
//            Request = (GenApi.PickCorectRequest(methodName))
//        };
//}
//public static class GenApi
//{
//    public static System.Random random { get; set; } = new System.Random(1234567);

//    public static object PickCorectRequest(MethodName methodName)
//    {
//        var res = methodName switch
//        {
//            MethodName.GetWeatherForecastAsync => 0,
//            MethodName.PetsPOSTAsync => new ArbitraryPet().Generator.Eval(1, FsCheck.Random.StdGen.NewStdGen(1, 2)), // PetsPOSTAsync expects a Pet as request
//            MethodName.PetsPUTAsync => new ArbitraryPet().Generator.Eval(1, FsCheck.Random.StdGen.NewStdGen(1, 2)),  // PetsPUTAsync expects a Pet as request
//            MethodName.PetsGETAsync => Gen.Choose(1, 100).Eval(1, FsCheck.Random.StdGen.NewStdGen(1, 2)),            // PetsGETAsync expects an int (likely ID)
//            MethodName.PetsAllAsync => (object)Gen.Constant(0).Eval(1, FsCheck.Random.StdGen.NewStdGen(1, 2)),                      // PetsAllAsync doesn't use a request, so you can use a default value or special marker (e.g., 0)
//            _ => throw new InvalidOperationException("Unknown method")
//        };

//        return res;
//    }

//    public static object PickCorrectRequestReflectionBasedWithExpression(string methodName, OneOf<Type, VoidEmptyType> methodParameterType)
//    {
//        var res = methodName switch
//        {
//            "GetWeatherForecastAsync" => CreateInstanceWithRandomValues(methodParameterType),
//            "PetsPOSTAsync" => CreateInstanceWithRandomValues(methodParameterType), // Create a random instance for PetsPOSTAsync
//            "PetsPUTAsync" => CreateInstanceWithRandomValues(methodParameterType),  // Create a random instance for PetsPUTAsync
//            "PetsGETAsync" => CreateInstanceWithRandomValues(methodParameterType),                       // PetsGETAsync expects an int (likely ID)
//            "PetsAllAsync" => CreateInstanceWithRandomValues(methodParameterType),                       // PetsAllAsync doesn't use a request, so use a default value
//            _ => throw new InvalidOperationException("Unknown method")
//        };

//        return res;
//    }

//    public static object PickCorrectRequestReflectionBased(string methodName, OneOf<Type, VoidEmptyType> methodParameterType)
//    {
//        var res = CreateInstanceWithRandomValues(methodParameterType);
//        return res;
//    }


//    private static object CreateInstanceWithRandomValues(OneOf<Type, VoidEmptyType> methodtype)
//    {
//        var instance = methodtype.Match(type => createInstance(type), voidType => null);

//        return instance;

//        static object createInstance(Type type)
//        {
//            var instance = Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Cannot create instance of type {type}");

//            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

//            if (properties?.Length == 0)
//            {
//                var randomValue = CreateRandomValue(type);
//                return randomValue;
//            }

//            // Set random values to the properties
//            foreach (var property in properties)
//            {
//                if (!property.CanWrite) continue; // Skip read-only properties
//                var randomValue = CreateRandomValue(property.PropertyType);
//                property.SetValue(instance, randomValue);
//            }

//            return instance;
//        }
//    }

//    private static object CreateRandomValue(Type type)
//    {
//        return type switch
//        {
//            Type t when t == typeof(int) => random.Next(1, 100),                          // Random integer between 1 and 100
//            Type t when t == typeof(string) => "RandomString" + random.Next(1, 1000),     // Random string with a number
//            Type t when t == typeof(bool) => random.Next(0, 2) == 0,                      // Random boolean
//            Type t when t == typeof(double) => random.NextDouble() * 100,                 // Random double between 0 and 100
//            Type t when t == typeof(DateTime) => DateTime.Now.AddDays(random.Next(-100, 100)), // Random date within +/- 100 days

//            // Handle enums by randomly selecting one of the possible values
//            Type t when t.IsEnum => GetRandomEnumValue(t, random),

//            // For complex types, recursively create an instance with random values
//            Type t when t.IsClass && t.GetConstructor(Type.EmptyTypes) != null => CreateInstanceWithRandomValues(t),

//            // If the type is not supported, return null
//            _ => throw new InvalidOperationException($@"Cant create '{type.ToString()}'")
//        };
//    }

//    // Helper method to get a random value from an enum type
//    private static object GetRandomEnumValue(Type enumType, System.Random random)
//    {
//        var values = Enum.GetValues(enumType);
//        var randomIndex = random.Next(values.Length);
//        return values.GetValue(randomIndex)!;
//    }


//}


//public class ArbitraryTestSuite : Arbitrary<TestSuite>
//{

//    public override Gen<TestSuite> Generator =>
//        from apiCalls in Gen.ListOf(new ArbitraryApiCall().Generator)
//        select new TestSuite
//        {
//            ApiCalls = apiCalls.ToList(),
//            TestCoveragePercentage = 0
//        };

//    public List<TestSuite> CreateList(DataModel dataModel)
//    {
//        //var list = dataModel.Methods.Select(item => new ApiCall
//        //{
//        //    methodName = item.MethodName.ToEnum<MethodName>(),
//        //    Request = (GenApi.PickCorectRequest(item.MethodName.ToEnum<MethodName>()))
//        //}).ToList();

//        var range = Enumerable.Range(1, dataModel.Methods.Count).ToList();
//        var listSuitePermutations = PermutationGenerator.GetPermutations(range);

//        var permuations = listSuitePermutations.Select(set =>
//        {
//            var list = set.Zip(dataModel.Methods).Select((item, index) => new ApiCall
//            {
//                Order = item.First,
//                methodName = item.Second.MethodName.ToEnum<MethodName>(),
//                Request = (GenApi.PickCorrectRequestReflectionBased(item.Second.MethodName, item.Second.MethodParameterType))
//            }).ToList();
//            return list.ToList();
//        }).ToList();



//        //var listSuitePermutations = PermutationGenerator.GetPermutations(list);
//        //var deep = DeepCopy(listSuitePermutations);

//        var testSuites = permuations.Select(calls => new TestSuite
//        {
//            ApiCalls = calls.ToList(),
//            //ApiCalls = calls.OrderBy(item => item.Order).ToList(),
//            TestCoveragePercentage = 0
//        }).ToList();
//        return testSuites;
//    }

//    public static List<List<ApiCall>> DeepCopy(List<List<ApiCall>> objectToCopy)
//    {

//        var options = new JsonSerializerOptions
//        {
//            WriteIndented = true, // Format the JSON with indentation
//            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Ignore null values
//        };

//        string json = System.Text.Json.JsonSerializer.Serialize(objectToCopy, options);
//        var res = System.Text.Json.JsonSerializer.Deserialize<List<List<ApiCall>>>(json);


//        foreach (var item in res)
//        {
//            foreach (var item1 in item)
//            {
//                item1.Request = (object)item1.Request;
//            }
//        }

//        return res;
//    }

//}



//public class CustomArgonJsonConverter : Argon.JsonConverter
//{
//    /// <summary>
//    /// Writes the JSON representation of the object.
//    /// </summary>
//    public override void WriteJson(Argon.JsonWriter writer, object value, Argon.JsonSerializer serializer)
//    {
//        if (value is IList list && list.Count == 0)
//        {
//            // If the list is empty, write it as an empty array
//            writer.WriteStartArray();
//            writer.WriteEndArray();
//        }
//        else if (value == null)
//        {
//            // Handle null values explicitly
//            writer.WriteNull();
//        }
//        else
//        {

//            // For all other cases, use the Argon serializer
//            serializer.Serialize(writer, value);
//        }
//    }

//    /// <summary>
//    /// Reads the JSON representation of the object.
//    /// </summary>
//    public override object? ReadJson(Argon.JsonReader reader, Type type, object? existingValue, Argon.JsonSerializer serializer)
//    {
//        // Handle null case
//        if (reader.Value is null)
//        {
//            return null;
//        }

//        // Deserialize the JSON into the specified type
//        return serializer.Deserialize(reader, type);
//    }

//    /// <summary>
//    /// Determines whether this instance can convert the specified object type.
//    /// </summary>
//    public override bool CanConvert(Type type)
//    {
//        // Allow any type to be converted

//        return true;
//    }

//}
//public static class EnumExtensions
//{
//    public static T ToEnum<T>(this string value) where T : struct, Enum
//    {
//        if (string.IsNullOrWhiteSpace(value))
//        {
//            throw new ArgumentException("String value cannot be null or empty.", nameof(value));
//        }

//        if (Enum.TryParse<T>(value, true, out var result))
//        {
//            return result;
//        }
//        else
//        {
//            throw new ArgumentException($"Cannot convert '{value}' to enum type '{typeof(T).Name}'.", nameof(value));
//        }
//    }

//    public static string GetDisplayName(this Enum enumValue)
//    {
//        // Get the type of the enum
//        var enumType = enumValue.GetType();

//        // Get the enum member information
//        var memberInfo = enumType.GetMember(enumValue.ToString());

//        if (memberInfo.Length > 0)
//        {
//            // Get the Display attribute if it exists
//            var displayAttribute = memberInfo[0].GetCustomAttribute<DisplayAttribute>();

//            if (displayAttribute != null)
//            {
//                // Return the Name property from the Display attribute if set
//                return displayAttribute.Name ?? enumValue.ToString();
//            }
//        }

//        // Return the enum name itself if no Display attribute is found
//        return enumValue.ToString();
//    }
//}