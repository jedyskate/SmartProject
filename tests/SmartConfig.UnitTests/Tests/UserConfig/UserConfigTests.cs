using AutoFixture;
using Moq;
using NUnit.Framework;
using Shouldly;
using SmartConfig.Application.Application.UserConfig.Commands;
using SmartConfig.Application.Application.UserConfig.Queries;
using SmartConfig.UnitTests.Infrastructure;

namespace SmartConfig.UnitTests.Tests.UserConfig;

[TestFixture]
public class UserConfigTests : TestBase
{
    [Test]
    public async Task CreateUserConfigCommand_Test()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();

        var handler = new CreateUserConfigCommand.Handler(_context, mockServiceProvider.Object);
        var method = new Fixture().Create<CreateUserConfigCommand>();

        var response = await handler.Handle(method, CancellationToken.None);
        response.Name.ShouldBe(method.Name);
    }

    [Test]
    public async Task GetUserConfigQuery_Test()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();

        var obj = new Fixture().Create<Core.Models.UserConfig>();
        obj.Identifier = "test-1010";

        _context.UserConfigs.AddRange(obj);
        await _context.SaveChangesAsync();

        var handler = new GetUserConfigQuery.Handler(_context, mockServiceProvider.Object);
        var method = new Fixture().Create<GetUserConfigQuery>();
        method.Identifier = "test-1010";

        var response = await handler.Handle(method, CancellationToken.None);
        response.Name.ShouldBe(obj.Name);
    }
}