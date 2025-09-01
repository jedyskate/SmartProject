using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace SmartConfig.Blazor.Client.Components
{
    public partial class Chat : IDisposable
    {
        private HttpClient _http;
        private PersistingComponentStateSubscription _persistingSubscription;

        private List<ChatMessage> messages = new();
        private string userInput = "";

        protected override async Task OnInitializedAsync()
        {
            _http = HttpClientFactory.CreateClient("n8n");
            _persistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);

            if (ApplicationState.TryTakeFromJson<ChatState>("chat_state", out var restoredState))
            {
                messages = restoredState.Messages;
                userInput = restoredState.UserInput;
            }

            await base.OnInitializedAsync();
        }

        private async Task SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(userInput))
            {
                messages.Add(new ChatMessage { Text = userInput, IsUser = true });
                StateHasChanged(); // Update the UI immediately with the user's message

                try
                {
                    var response = await _http.PostAsJsonAsync("webhook/smartconfig-chat", new { message = userInput });
                    if (response.IsSuccessStatusCode)
                    {
                        var botResponse = await response.Content.ReadFromJsonAsync<List<ChatResponse>>();
                        if (botResponse is { Count: > 0 })
                        {
                            messages.Add(new ChatMessage { Text = botResponse[0].Text, IsUser = false });
                        }
                        else
                        {
                            messages.Add(new ChatMessage { Text = "Received an empty response from the bot.", IsUser = false });
                        }
                    }
                    else
                    {
                        messages.Add(new ChatMessage { Text = "Error: Could not get a response from the bot.", IsUser = false });
                    }
                }
                catch (Exception ex)
                {
                    messages.Add(new ChatMessage { Text = $"An error occurred: {ex.Message}", IsUser = false });
                }

                userInput = "";
                StateHasChanged();
            }
        }

        private async Task HandleKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await SendMessage();
            }
        }

        // This method is called by the framework to save the state
        private Task PersistData()
        {
            // Bundle the data into the state object and save it
            var state = new ChatState
            {
                Messages = messages,
                UserInput = userInput
            };
            ApplicationState.PersistAsJson("chat_state", state);
            
            return Task.CompletedTask;
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
        public bool IsUser { get; set; }
    }

    public class ChatResponse
    {
        public string? Text { get; set; }
    }
}