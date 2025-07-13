using AutoFixture;
using NUnit.Framework;
using Shouldly;
using SmartConfig.IntegrationTests.Infrastructure;
using SmartConfig.Sdk;

namespace SmartConfig.IntegrationTests.Tests;

[TestFixture, Order(2)]
public class UserConfigTests : TestBase
{
    private static string Identifier { get; set; }

    [Test, Order(10)]
    public async Task CreateUserConfig_Test()
    {
        var input = new Fixture().Create<CreateUserConfigCommand>();
        input.Options.CreatePreferences = true;
        input.Options.CreateSettings = true;

        var response = await SmartConfigClient.CreateUserConfigAsync(input);
        response.Response.Identifier.ShouldNotBeNullOrEmpty();

        Identifier = response.Response.Identifier;
    }

    [Test, Order(20)]
    public async Task GetUserConfig_Test()
    {
        var response = await SmartConfigClient.GetUserConfigAsync(new GetUserConfigQuery
        {
            Identifier = Identifier
        });
        response.Response.Identifier.ShouldBe(Identifier);
    }
}