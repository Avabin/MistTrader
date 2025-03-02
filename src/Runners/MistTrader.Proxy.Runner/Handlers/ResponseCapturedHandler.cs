using MediatR;
using Microsoft.Extensions.Logging;
using MistTrader.Proxy.Notifications;

namespace MistTrader.Proxy.Runner.Handlers;

internal class ResponseCapturedHandler(ILogger<ResponseCapturedHandler> logger) : INotificationHandler<ResponseCaptured>
{
    private readonly ILogger<ResponseCapturedHandler> _logger = logger;

    public Task Handle(ResponseCaptured notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Captured response from {Url}", notification.Url);
        return Task.CompletedTask;
    }
}