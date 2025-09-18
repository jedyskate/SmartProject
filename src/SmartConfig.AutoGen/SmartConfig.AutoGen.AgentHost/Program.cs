using Microsoft.Extensions.Hosting;
using Host = SmartConfig.AgentHost.Host;

var app = await Host.StartAsync(local: false, useGrpc: true).ConfigureAwait(false);
await app.WaitForShutdownAsync();
