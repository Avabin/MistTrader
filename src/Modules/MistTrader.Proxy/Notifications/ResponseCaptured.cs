using MediatR;

namespace MistTrader.Proxy.Notifications;

public readonly record struct JsonResponseCaptured(Uri Url, string Response, DateTimeOffset Timestamp) : INotification;
public readonly record struct ImageResponseCaptured(Uri Url, ReadOnlyMemory<byte> Response, DateTimeOffset Timestamp) : INotification;