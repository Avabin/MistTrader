using System.Text;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MistTrader.Proxy.Models;
using MistTrader.Proxy.Notifications;

namespace MistTrader.Proxy.Services;

internal class MistwoodProxyCaptureEventPublisher(IMistwoodProxy proxy, IMediator mediator, ILogger<MistwoodProxyCaptureEventPublisher> logger) : IHostedService
{
    private readonly IMistwoodProxy _proxy = proxy;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<MistwoodProxyCaptureEventPublisher> _logger = logger;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting proxy event publisher");
        _proxy.ResponseInterceptedAsync += OnProxyResponseInterceptedAsync;
        return Task.CompletedTask;
    }

    private async Task OnProxyResponseInterceptedAsync(MistwoodResponseEventArgs e)
    {
        _logger.LogDebug("Response intercepted from {Url}", e.RequestUrl);
        if (!e.Body.HasValue || e.Body.Value.Length == 0)
        {
            _logger.LogWarning("Empty response body from {Url}", e.RequestUrl);
            return;
        }

        var castedBody = e.Body.Value;
        INotification notification = e.IsImage ? new ImageResponseCaptured(e.RequestUrl, castedBody, e.Timestamp) :
            new JsonResponseCaptured(e.RequestUrl, Encoding.UTF8.GetString(castedBody.Span), e.Timestamp);
        
        await _mediator.Publish(notification);
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping proxy event publisher");
        _proxy.ResponseInterceptedAsync -= OnProxyResponseInterceptedAsync;
        return Task.CompletedTask;
    }
}