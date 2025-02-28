using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MistTrader.Proxy;
using MistTrader.Proxy.Commands;
using MistTrader.Proxy.Runner.Handlers;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddProxy().AddMediatR(c => c.RegisterServicesFromAssemblyContaining<ProxyStartedHandler>());

var app = builder.Build();

await app.StartAsync();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

var mediator = app.Services.GetRequiredService<IMediator>();
var startCmd = new StartProxy(8080);
await mediator.Send(startCmd);

logger.LogInformation("Proxy started on port {Port}", startCmd.Port);

Console.WriteLine("Press any key to stop the proxy...");

Console.ReadKey();

var stopCmd = new StopProxy();

await mediator.Send(stopCmd);

logger.LogInformation("Proxy stopped");

await app.StopAsync();