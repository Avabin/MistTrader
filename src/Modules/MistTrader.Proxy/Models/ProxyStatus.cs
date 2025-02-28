namespace MistTrader.Proxy.Models;

public record ProxyStatus(bool IsRunning, ushort Port, ulong TotalRequests);