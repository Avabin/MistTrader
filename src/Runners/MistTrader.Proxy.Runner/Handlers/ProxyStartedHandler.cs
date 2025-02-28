using MediatR;
using Microsoft.Extensions.Logging;
using MistTrader.Proxy.Notifications;

namespace MistTrader.Proxy.Runner.Handlers;

internal class ProxyStartedHandler(ILogger<ProxyStartedHandler> logger) : INotificationHandler<ProxyStarted>
{
    private readonly ILogger<ProxyStartedHandler> _logger = logger;
    
    public Task Handle(ProxyStarted notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Proxy started on port {Port}", notification.Port);
        
        return Task.CompletedTask;
    }
}