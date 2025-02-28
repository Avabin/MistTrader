using MediatR;

namespace MistTrader.Proxy.Notifications;

public readonly record struct ProxyStarted : INotification
{
    public int Port { get; }

    public ProxyStarted(int port)
    {
        Port = port;
    }
}