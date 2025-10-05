using NUnit.Framework;
using Shouldly;
using SmartConfig.IntegrationTests.Infrastructure;
using SmartConfig.BE.Sdk;

namespace SmartConfig.IntegrationTests.Tests;

[TestFixture, Order(3)]
public class OrleansTests : TestBase
{
    [Test, Order(10)]
    public async Task FirstHelloWorld_Test()
    {
        var smartConfigClient = SmartConfigApiClient;
            
        // First hello
        var firstCommand = new HelloWorldCommand { Name = "Tester" };
        var firstResponse = await smartConfigClient.HelloWorldAsync(firstCommand);
        firstResponse.Response.ShouldBe("Hello world number 1 from Tester. Total hello world count: 1");
        
        // Second hello
        var secondCommand = new HelloWorldCommand { Name = "User" };
        var secondResponse = await smartConfigClient.HelloWorldAsync(secondCommand);
        secondResponse.Response.ShouldBe("Hello world number 1 from User. Total hello world count: 2");
        
        // Third hello
        var thirdCommand = new HelloWorldCommand { Name = "Tester" };
        var thirdResponse = await SmartConfigApiClient.HelloWorldAsync(thirdCommand);
        thirdResponse.Response.ShouldBe("Hello world number 2 from Tester. Total hello world count: 3");
    }
}
