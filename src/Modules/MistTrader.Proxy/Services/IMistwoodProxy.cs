using MistTrader.Proxy.Models;

namespace MistTrader.Proxy.Services;

/// <summary>
/// Interface for handling Mistwood game traffic interception
/// </summary>
internal interface IMistwoodProxy
{
    /// <summary>
    /// Current status of the proxy
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Port number the proxy is listening on
    /// </summary>
    ushort Port { get; }

    /// <summary>
    /// Total number of requests intercepted
    /// </summary>
    ulong TotalRequests { get; }

    /// <summary>
    /// Event raised when a request to mistwood.pl is intercepted
    /// </summary>
    event Func<MistwoodRequestEventArgs, Task> RequestInterceptedAsync;

    /// <summary>
    /// Event raised when a response from mistwood.pl is intercepted
    /// </summary>
    event Func<MistwoodResponseEventArgs, Task> ResponseInterceptedAsync;

    /// <summary>
    /// Starts the proxy server
    /// </summary>
    /// <param name="port">Port to listen on. Default is 8000</param>
    Task StartAsync(int port = 8000, CancellationToken cancellationToken = default);
    
    /// <inheritdoc cref="StartAsync(int, CancellationToken)"/>
    void Start(int port = 8000);

    /// <summary>
    /// Stops the proxy server
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);
    
    /// <inheritdoc cref="StopAsync(CancellationToken)"/>
    void Stop();

    /// <summary>
    /// Clears all counters and internal state
    /// </summary>
    void Reset();
}