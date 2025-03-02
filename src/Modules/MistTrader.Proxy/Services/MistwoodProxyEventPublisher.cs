using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MistTrader.Proxy.Models;
using MistTrader.Proxy.Notifications;

namespace MistTrader.Proxy.Services;

internal class MistwoodProxyEventPublisher(IMistwoodProxy proxy, IMediator mediator, ILogger<MistwoodProxyEventPublisher> logger) : IHostedService
{
    private readonly IMistwoodProxy _proxy = proxy;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<MistwoodProxyEventPublisher> _logger = logger;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting proxy event publisher");
        _proxy.ResponseInterceptedAsync += OnProxyResponseInterceptedAsync;
        return Task.CompletedTask;
    }

    private async Task OnProxyResponseInterceptedAsync(MistwoodResponseEventArgs e)
    {
        _logger.LogDebug("Response intercepted from {Url}", e.RequestUrl);
        await _mediator.Publish(new ResponseCaptured(e.RequestUrl, e.Body ?? ""));
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping proxy event publisher");
        _proxy.ResponseInterceptedAsync -= OnProxyResponseInterceptedAsync;
        return Task.CompletedTask;
    }
}