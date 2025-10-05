using System.Reflection;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NUnit.Framework;
using SmartConfig.IntegrationTests.Infrastructure;

namespace SmartConfig.IntegrationTests.Tests;

[TestFixture, Order(1)]
public class SwaggerTests : TestBase
{
    [Test, Order(10)]
    public async Task CreateProxyClient_Test()
    {
        Console.WriteLine("Sdk tool initialized");

        await GenerateRestSdk();

        Console.WriteLine("Sdk tool finalized");
    }

    private static async Task GenerateRestSdk()
    {
        var document = await GetOpenApiDocument();

        Console.WriteLine("Rest C# client generator initialized");

        var targetFileName = "SmartConfigClient.cs";
        var @namespace = "SmartConfig.BE.Sdk";
        var outputPath = GetSdkOutputPath(@namespace);

        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = "SmartConfigClient",
            CSharpGeneratorSettings =
            {
                Namespace = @namespace,
                RequiredPropertiesMustBeDefined = false
            },
            GenerateClientInterfaces = true,
            UseBaseUrl = false,
            ExposeJsonSerializerSettings = true
        };

        var generator = new CSharpClientGenerator(document, settings);
        var code = generator.GenerateFile();

        Console.WriteLine("Code extracted from rest endpoint");
        var result = GenerateRestClient(code, targetFileName, outputPath);
        if (result)
        {
            Console.WriteLine("Rest SDK created and exported");
            Console.WriteLine("Rest C# client generator finalized");
        }
        else
        {
            Console.WriteLine("Rest C# client generator ended with errors");
        }
    }

    private static async Task<OpenApiDocument> GetOpenApiDocument()
    {
        try
        {
            Console.WriteLine("Initializing API instance in memory");

            var factory = new CustomWebApplicationFactory<Api.Program>();
            using var client = factory.CreateClient();

            Console.WriteLine("Fetching open Api document from API instance");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/swagger/v1/swagger.json");
            var response = await client.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            await factory.DisposeAsync();
            var document = await OpenApiDocument.FromJsonAsync(responseText, CancellationToken.None);

            Console.WriteLine("Open Api document generated");

            return document;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static string GetSdkOutputPath(string @namespace)
    {
        string solutionPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location)!
                                  .Parent?.Parent?.Parent?.Parent?.Parent?.FullName +
                              $"{Path.DirectorySeparatorChar}sdk";

        if (solutionPath == string.Empty)
            return string.Empty;

        var outputPath = solutionPath + Path.DirectorySeparatorChar + @namespace;

        Console.WriteLine($"Output path: {outputPath}");

        return outputPath;
    }

    private static bool GenerateRestClient(string cSharpCode, string targetFileName, string outputPath)
    {
        try
        {
            using StreamWriter outputFile = new StreamWriter(outputPath + Path.DirectorySeparatorChar + targetFileName);
            outputFile.WriteLine(cSharpCode);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating SDK. Details: " + ex);

            return false;
        }
    }
}