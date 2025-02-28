using MediatR;
using Microsoft.Extensions.Logging;
using MistTrader.Proxy.Notifications;

namespace MistTrader.Proxy.Runner.Handlers;

internal class ProxyStoppedHandler(ILogger<ProxyStoppedHandler> logger) : INotificationHandler<ProxyStopped>
{
    private readonly ILogger<ProxyStoppedHandler> _logger = logger;
    
    public Task Handle(ProxyStopped notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Proxy stopped");
        
        return Task.CompletedTask;
    }
}