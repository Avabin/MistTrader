using MediatR;

namespace MistTrader.Proxy.Notifications;

public record ResponseCaptured(Uri Url, string Response) : INotification;