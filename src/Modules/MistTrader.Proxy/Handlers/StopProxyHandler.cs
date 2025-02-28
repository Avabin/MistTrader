using MediatR;
using MistTrader.Proxy.Commands;
using MistTrader.Proxy.Notifications;
using MistTrader.Proxy.Services;

namespace MistTrader.Proxy.Handlers;

internal class StopProxyHandler(IMistwoodProxy proxy, IMediator mediator) : IRequestHandler<StopProxy>
{
    private readonly IMistwoodProxy _proxy = proxy;
    private readonly IMediator _mediator = mediator;

    public async Task Handle(StopProxy command, CancellationToken cancellationToken)
    {
        await _proxy.StopAsync(cancellationToken);
        
        await _mediator.Publish(new ProxyStopped(), cancellationToken);
    }
}