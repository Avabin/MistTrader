using MediatR;

namespace MistTrader.Proxy.Notifications;

public readonly record struct ResponseCaptured(Uri Url, string Response, DateTimeOffset Timestamp) : INotification;