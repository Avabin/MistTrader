using MediatR;
using MistTrader.Proxy.Commands;
using MistTrader.Proxy.Notifications;
using MistTrader.Proxy.Services;

namespace MistTrader.Proxy.Handlers;

internal sealed class StartProxyHandler(IMistwoodProxy proxy, IMediator mediator) : IRequestHandler<StartProxy>
{
    private readonly IMistwoodProxy _proxy = proxy;
    private readonly IMediator _mediator = mediator;

    public async Task Handle(StartProxy command, CancellationToken cancellationToken)
    {
        await _proxy.StartAsync(command.Port, cancellationToken);
        
        await _mediator.Publish(new ProxyStarted(command.Port), cancellationToken);
    }
}