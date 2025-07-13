using Moq;
using NUnit.Framework;
using Shouldly;
using SmartConfig.Orleans.Silo.Grains.Tests;

namespace SmartConfig.UnitTests.Tests.Orleans.Grains;

[TestFixture]
public class HelloWorldGrainTests
{
    [Test]
    public async Task SayHelloWorld_Should_Return_Correct_Message()
    {
        // Arrange
        var grainFactoryMock = new Mock<IGrainFactory>();
        var persistentStateMock = new Mock<IPersistentState<HelloWorldState>>();
        var helloCounterTotalGrainMock = new Mock<IHelloCounterTotalGrain>();

        var state = new HelloWorldState();
        persistentStateMock.Setup(x => x.State).Returns(state);
        persistentStateMock.Setup(x => x.WriteStateAsync()).Returns(Task.CompletedTask);

        grainFactoryMock.Setup(x => x.GetGrain<IHelloCounterTotalGrain>(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(helloCounterTotalGrainMock.Object);

        helloCounterTotalGrainMock.Setup(x => x.IncreaseHelloCounter()).ReturnsAsync(1);

        var grain = new HelloWorldGrain(grainFactoryMock.Object, persistentStateMock.Object);
        var name = "John";

        // Act
        var result = await grain.SayHelloWorld(name);

        // Assert
        result.ShouldBe($"Hello world number 1 from {name}. Total hello world count: 1");
        persistentStateMock.Verify(x => x.WriteStateAsync(), Times.Once);
    }

    [Test]
    public async Task IncreaseHelloCounter_Should_Increment_Count()
    {
        // Arrange
        var persistentStateMock = new Mock<IPersistentState<HelloCounterState>>();
        var state = new HelloCounterState();
        persistentStateMock.Setup(x => x.State).Returns(state);
        persistentStateMock.Setup(x => x.WriteStateAsync()).Returns(Task.CompletedTask);

        var grain = new HelloCounterTotalGrain(persistentStateMock.Object);

        // Act
        var result = await grain.IncreaseHelloCounter();

        // Assert
        result.ShouldBe(1);
        state.Count.ShouldBe(1);
        persistentStateMock.Verify(x => x.WriteStateAsync(), Times.Once);
    }
}
