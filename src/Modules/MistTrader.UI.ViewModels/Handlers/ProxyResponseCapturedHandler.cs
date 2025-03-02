using MediatR;
using MistTrader.Proxy.Notifications;
using ReactiveUI;

namespace MistTrader.UI.ViewModels.Handlers;

public class ProxyResponseCapturedHandler(IMessageBus bus) : INotificationHandler<ResponseCaptured>
{
    private readonly IMessageBus _bus = bus;

    public Task Handle(ResponseCaptured notification, CancellationToken cancellationToken)
    {
        _bus.SendMessage(notification);
        return Task.CompletedTask;
    }
}