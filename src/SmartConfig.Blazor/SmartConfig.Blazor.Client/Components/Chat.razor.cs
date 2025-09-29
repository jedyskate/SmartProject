using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using shortid;
using SmartConfig.Sdk;


namespace SmartConfig.Blazor.Client.Components;

public partial class Chat : IDisposable
{
    private DotNetObjectReference<Chat> _dotNetRef;
    private ElementReference _chatBody;
    private PersistingComponentStateSubscription _persistingSubscription;

    private List<ChatMessage> _messages = new();
    private string _userInput = "";
    private string _currentAgentMessageId = "";

    protected override async Task OnInitializedAsync()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        _persistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);

        if (ApplicationState.TryTakeFromJson<ChatState>("chat_state", out var restoredState))
        {
            _messages = restoredState.Messages;
            _userInput = restoredState.UserInput;
        }

        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JSRuntime.InvokeVoidAsync("scrollToBottom", _chatBody);
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(_userInput)) return;

        _messages.Add(new ChatMessage { Id = ShortId.Generate(), Text = _userInput, IsUser = true });
        var chatRequest = new
        {
            messages = new[]
            {
                new { role = "user", content = _userInput }
            }
        };

        _userInput = string.Empty;
        _currentAgentMessageId = ShortId.Generate();
        StateHasChanged();

        // BLAZOR WASM STREAMING DOESN'T PROPERLY WORK DUE TO THE HTTPCLIENT.
        // This is an alternative solution.
        var url = $"{Configuration["SmartConfig:ApiEndpoint"]}/api/AiAgent/CompleteChatStreaming";
        await JSRuntime.InvokeVoidAsync("streamChat", url, chatRequest, _dotNetRef);
    }

    [JSInvokable]
    public void ProcessStreamChunk(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return;

        var agentStream = TryDeserializeStream(line);
        var existingMessage = _messages.FirstOrDefault(r => r.Id == _currentAgentMessageId);
        if (existingMessage != null)
        {
            existingMessage.Text += agentStream?.Content;
        }
        else
        {
            _messages.Add(new ChatMessage { Id = _currentAgentMessageId, Text = agentStream?.Content, IsUser = false });
        }

        InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public void StreamCompleted()
    {
        // You can add any logic here that needs to run when the stream is complete.
    }

    [JSInvokable]
    public void StreamFailed(string error)
    {
        _messages.Add(new ChatMessage { Id = ShortId.Generate(), Text = $"Error: {error}", IsUser = false });
        InvokeAsync(StateHasChanged);
    }

    private ChatResponse? TryDeserializeStream(string line)
    {
        ChatResponse? agentStream = null;
        try
        {
            agentStream = JsonConvert.DeserializeObject<ChatResponse>(line);
        }
        catch (JsonException jex)
        {
            _messages.Add(new ChatMessage
            {
                Id = ShortId.Generate(),
                Text = $"JSON parse error: {jex.Message}",
                IsUser = false
            });
            return agentStream;
        }

        return agentStream;
    }

    // This method is called by the framework to save the state
    private Task PersistData()
    {
        // Bundle the data into the state object and save it
        var state = new ChatState
        {
            Messages = _messages,
            UserInput = _userInput
        };
        ApplicationState.PersistAsJson("chat_state", state);

        return Task.CompletedTask;
    }

    private async Task OnKeyDownAsync(KeyboardEventArgs e)
    {
        if (e.Key != "Enter")
        {
            return;
        }

        await SendMessage();
    }

    public void Dispose()
    {
        _persistingSubscription.Dispose();
        _dotNetRef?.Dispose();
    }
}

public class ChatState
{
    public List<ChatMessage> Messages { get; set; } = new();
    public string UserInput { get; set; } = "";
}

public class ChatMessage
{
    public string Id { get; set; }
    public string? Text { get; set; }
    public bool? IsUser { get; set; }
}