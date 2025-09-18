using AutoGen.Core;
using AutoGen.Ollama;
using AutoGen.Ollama.Extension;

// Step 1: Create a new HttpClient instance for the Ollama server.
// The BaseAddress points to the local Ollama API endpoint.
using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:11434");

// Step 2: Create an OllamaAgent instance.
// This is the dedicated agent for connecting to Ollama.
var ollamaAgent = new OllamaAgent(
        httpClient: httpClient,
        name: "ollama-agent",
        modelName: "llama3.2", // The model name you pulled in Ollama.
        systemMessage: "You are a helpful and polite AI assistant that introduces yourself."
    )
    .RegisterMessageConnector()
    .RegisterPrintMessage(); // Optional, but useful for debugging

// Step 3: Start a conversation with the agent.
// The `SendAsync` method initiates the chat.
var reply = await ollamaAgent.SendAsync("Hello!");

// Step 4: Print the agent's reply.
if (reply is TextMessage textMessage)
{
    Console.WriteLine($"\nOllama Agent's Reply: {textMessage.Content}");
}
else
{
    Console.WriteLine("Received a non-text message or an empty reply.");
}