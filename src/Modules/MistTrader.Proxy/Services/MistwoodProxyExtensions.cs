using MistTrader.Proxy.Models;

namespace MistTrader.Proxy.Services;

public static class MistwoodProxyExtensions
{
    internal static ProxyStatus ToStatus(this IMistwoodProxy proxy) => 
        new(proxy.IsRunning, proxy.Port, proxy.TotalRequests);
}