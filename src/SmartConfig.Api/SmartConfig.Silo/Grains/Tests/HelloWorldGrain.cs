namespace SmartConfig.Silo.Grains.Tests;

public interface IHelloWorldUserGrain : IGrainWithStringKey
{
    Task<string> SayHelloWorld(string name);
}

public class HelloWorldState
{
    public int HelloWorldCount { get; set; } = 0;
}
    
public class HelloWorldGrain : Grain, IHelloWorldUserGrain
{
    private readonly IGrainFactory _grainFactory;
    private readonly IPersistentState<HelloWorldState> _state;

    public HelloWorldGrain(IGrainFactory grainFactory, [PersistentState("helloWorldState")] IPersistentState<HelloWorldState> state)
    {
        _grainFactory = grainFactory;
        _state = state;
    }

    public async Task<string> SayHelloWorld(string name)
    {
        //throw new SmartConfigException(HttpStatusCode.InternalServerError, "Hello World, not allowed!");

        var grain = _grainFactory.GetGrain<IHelloCounterTotalGrain>("Generic-Grain-Identifier");
        var helloTotalCount = await grain.IncreaseHelloCounter();

        _state.State.HelloWorldCount++;
        await _state.WriteStateAsync();

        var persona = string.IsNullOrEmpty(name) ? "Nobody" : name;

        return $"Hello world number {_state.State.HelloWorldCount} from {persona}. Total hello world count: {helloTotalCount}";
    }
}