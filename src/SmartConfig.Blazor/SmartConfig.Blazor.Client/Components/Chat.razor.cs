using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;


namespace SmartConfig.Blazor.Client.Components
{
    public partial class Chat : IDisposable
    {
        private HttpClient _http;
        private ElementReference _chatBody;
        private PersistingComponentStateSubscription _persistingSubscription;

        private List<ChatMessage> _messages = new();
        private string _userInput = "";

        protected override async Task OnInitializedAsync()
        {
            _http = HttpClientFactory.CreateClient("n8n");
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

            _messages.Add(new ChatMessage { Text = _userInput, IsUser = true });
            var chatRequest = new { message = _userInput };

            _userInput = string.Empty;
            StateHasChanged();

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "webhook/smartconfig-chat")
                {
                    Content = JsonContent.Create(chatRequest)
                };

                // important for streaming
                var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    await using var stream = await response.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(stream);

                    await using var jsonReader = new JsonTextReader(reader);
                    var serializer = new JsonSerializer();
                    while (await jsonReader.ReadAsync())
                    {
                        if (jsonReader.TokenType != JsonToken.StartObject) continue;
                        
                        var message = serializer.Deserialize<AgentResponse>(jsonReader);
                        _messages.Add(new ChatMessage { Text = message?.Text, IsUser = false });
                        StateHasChanged();
                    }
                }
                else
                {
                    _messages.Add(new ChatMessage { Text = "Error: Bot response failed.", IsUser = false });
                }
            }
            catch (Exception ex)
            {
                _messages.Add(new ChatMessage { Text = $"Error: {ex.Message}", IsUser = false });
            }
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
        
        protected async Task OnKeyDownAsync(KeyboardEventArgs e)
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
        }
    }
    
    public class ChatState
    {
        public List<ChatMessage> Messages { get; set; } = new();
        public string UserInput { get; set; } = "";
    }
    
    public class ChatMessage
    {
        public string? Text { get; set; }
        public bool? IsUser { get; set; }
    }

    public class AgentResponse
    {
        public string? Text { get; set; }
    }
}