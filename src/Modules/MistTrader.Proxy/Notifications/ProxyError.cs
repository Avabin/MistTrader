using MediatR;

namespace MistTrader.Proxy.Notifications;

public record ProxyError(ProxyErrorType ErrorType, string Message) : INotification;