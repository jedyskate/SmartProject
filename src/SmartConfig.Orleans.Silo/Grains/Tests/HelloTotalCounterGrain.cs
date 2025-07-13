namespace SmartConfig.Orleans.Silo.Grains.Tests;

public interface IHelloCounterTotalGrain : IGrainWithStringKey
{
    Task<int> IncreaseHelloCounter();
}

public class HelloCounterState
{
    public int Count { get; set; } = 0;
}
    
public class HelloCounterTotalGrain : Grain, IHelloCounterTotalGrain
{
    private readonly IPersistentState<HelloCounterState> _state;

    public HelloCounterTotalGrain([PersistentState("helloCounterState")] IPersistentState<HelloCounterState> state)
    {
        _state = state;
    }

    public async Task<int> IncreaseHelloCounter()
    {
        _state.State.Count++;
        await _state.WriteStateAsync();

        return _state.State.Count;
    }
}