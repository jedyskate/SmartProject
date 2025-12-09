using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using shortid;

namespace SmartConfig.App.Shared.Components;

public partial class N8nChat : IDisposable
{
    private DotNetObjectReference<N8nChat> _dotNetRef;
    private ElementReference _chatBody;

    private List<ChatMessage> _messages = new();
    private string _userInput = "";
    private string _currentAgentMessageId = "";

    protected override async Task OnInitializedAsync()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
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
            message = _userInput
        };

        _userInput = string.Empty;
        _currentAgentMessageId = ShortId.Generate();
        StateHasChanged();

        // Use YARP proxy endpoint for n8n webhook
        var url = "/n8n/webhook/n8n/smartconfig-chat";

        await JSRuntime.InvokeVoidAsync("streamChat", url, chatRequest, _dotNetRef);
    }

    [JSInvokable]
    public void ProcessStreamChunk(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return;

        try
        {
            // Parse n8n streaming JSON format
            var jsonObj = JObject.Parse(line);
            var type = jsonObj["type"]?.ToString();

            // Only process "item" type chunks that contain content
            if (type == "item")
            {
                var content = jsonObj["content"]?.ToString();
                if (!string.IsNullOrEmpty(content))
                {
                    var existingMessage = _messages.FirstOrDefault(r => r.Id == _currentAgentMessageId);
                    if (existingMessage != null)
                    {
                        existingMessage.Text += content;
                    }
                    else
                    {
                        _messages.Add(new ChatMessage { Id = _currentAgentMessageId, Text = content, IsUser = false });
                    }

                    InvokeAsync(StateHasChanged);
                }
            }
        }
        catch (JsonException)
        {
            // Ignore malformed JSON chunks
        }
    }

    [JSInvokable]
    public void StreamCompleted()
    {
        // Stream is complete
    }

    [JSInvokable]
    public void StreamFailed(string error)
    {
        _messages.Add(new ChatMessage { Id = ShortId.Generate(), Text = $"Error: {error}", IsUser = false });
        InvokeAsync(StateHasChanged);
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
        _dotNetRef?.Dispose();
    }
}
