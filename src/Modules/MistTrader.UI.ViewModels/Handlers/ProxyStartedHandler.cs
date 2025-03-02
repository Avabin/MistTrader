using MediatR;
using MistTrader.Proxy.Notifications;
using ReactiveUI;

namespace MistTrader.UI.ViewModels.Handlers;

internal class ProxyStartedHandler : INotificationHandler<ProxyStarted>
{
    private readonly IMessageBus _bus;

    public ProxyStartedHandler(IMessageBus bus)
    {
        _bus = bus;
    }

    public Task Handle(ProxyStarted notification, CancellationToken cancellationToken)
    {
        _bus.SendMessage(notification);
        return Task.CompletedTask;
    }
}