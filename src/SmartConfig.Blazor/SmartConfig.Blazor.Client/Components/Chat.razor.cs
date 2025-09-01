using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Web;

namespace SmartConfig.Blazor.Client.Components
{
    public partial class Chat
    {
        private HttpClient _http;

        private List<ChatMessage> messages = new();
        private string userInput = "";

        protected override void OnInitialized()
        {
            _http = HttpClientFactory.CreateClient("n8n");
        }

        private async Task SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(userInput))
            {
                messages.Add(new ChatMessage { Text = userInput, IsUser = true });

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