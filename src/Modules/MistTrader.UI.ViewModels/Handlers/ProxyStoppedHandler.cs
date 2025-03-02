using MediatR;
using MistTrader.Proxy.Notifications;
using ReactiveUI;

namespace MistTrader.UI.ViewModels.Handlers;

internal class ProxyStoppedHandler : INotificationHandler<ProxyStopped>
{
    private readonly IMessageBus _bus;

    public ProxyStoppedHandler(IMessageBus bus)
    {
        _bus = bus;
    }

    public Task Handle(ProxyStopped notification, CancellationToken cancellationToken)
    {
        _bus.SendMessage(notification);
        return Task.CompletedTask;
    }
}