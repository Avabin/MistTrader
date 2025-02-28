using System.Net;
using MistTrader.Proxy.Exceptions;
using MistTrader.Proxy.Models;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace MistTrader.Proxy.Services;

internal sealed class MistwoodProxy : IMistwoodProxy, IAsyncDisposable
{
    private readonly ProxyServer _proxyServer;
    private ExplicitProxyEndPoint? _endPoint;
    private ulong _totalRequests;
    private bool _wasSystemProxy;
    private bool _disposed;
        
    public bool IsRunning { get; private set; }

    public ushort Port => Convert.ToUInt16(_endPoint?.Port ?? 0);
    public ulong TotalRequests => _totalRequests;

    public event Func<MistwoodRequestEventArgs, Task>? RequestInterceptedAsync;
    public event Func<MistwoodResponseEventArgs, Task>? ResponseInterceptedAsync;

    public MistwoodProxy()
    {
        _proxyServer = new ProxyServer();
            
        _proxyServer.BeforeRequest += OnRequestAsync;
        _proxyServer.BeforeResponse += OnResponseAsync;
            
        // Certificate generation
        _proxyServer.CertificateManager.CreateRootCertificate();
        _proxyServer.CertificateManager.TrustRootCertificate();
    }

    public async Task StartAsync(int port = 8000, CancellationToken cancellationToken = default) => await Task.Run(() => Start(port), cancellationToken);

    public void Start(int port = 8000)
    {
        ThrowIfDisposed();
        
        if (IsRunning)
            throw new InvalidOperationException("Proxy is already running");

        try
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(port, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(port, 65535);
            
            if (_endPoint is not null)
            {
                _proxyServer.RemoveEndPoint(_endPoint);
            }

            _endPoint = new ExplicitProxyEndPoint(IPAddress.Any, port, true);
            
            
            _proxyServer.AddEndPoint(_endPoint);
            _proxyServer.Start();
                
            // Set as system proxy
            _proxyServer.SetAsSystemProxy(_endPoint, ProxyProtocolType.Https);
            _wasSystemProxy = true;
                
            IsRunning = true;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new ProxyStartException("Failed to start proxy", ex);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default) => await Task.Run(Stop, cancellationToken);

    public void Stop()
    {
        ThrowIfDisposed();
        
        if (!IsRunning)
            return;

        if (_wasSystemProxy)
        {
            _proxyServer.RestoreOriginalProxySettings();
            _wasSystemProxy = false;
        }

        _proxyServer.Stop();
        IsRunning = false;
    }

    public void Reset()
    {
        ThrowIfDisposed();
        
        Interlocked.Exchange(ref _totalRequests, 0);
    }

    private async Task OnRequestAsync(object sender, SessionEventArgs e)
    {
        var uri = e.HttpClient.Request.RequestUri;
            
        if (!ShouldIntercept(uri)) return;
        
        Interlocked.Increment(ref _totalRequests);

        var headers = e.HttpClient.Request.Headers
            .ToDictionary(h => h.Name, h => string.Join(";", h.Value));
            
        var body = await e.GetRequestBodyAsString();
            
        var args = new MistwoodRequestEventArgs(
            uri,
            e.HttpClient.Request.Method,
            headers,
            body
        );

        if (RequestInterceptedAsync is not null)
        {
            var handlers = RequestInterceptedAsync.GetInvocationList()
                .Cast<Func<MistwoodRequestEventArgs, Task>>();
                    
            await Task.WhenAll(handlers.Select(handler => handler(args)));
        }
    }

    private async Task OnResponseAsync(object sender, SessionEventArgs e)
    {
        var uri = e.HttpClient.Request.RequestUri;
            
        if (!ShouldIntercept(uri)) return;
        
        var headers = e.HttpClient.Response.Headers
            .ToDictionary(h => h.Name, h => string.Join(";", h.Value));
            
        var body = await e.GetResponseBodyAsString();
            
        var args = new MistwoodResponseEventArgs(
            uri,
            (int)e.HttpClient.Response.StatusCode,
            headers,
            body
        );
            
        if (ResponseInterceptedAsync is not null)
        {
            var handlers = ResponseInterceptedAsync.GetInvocationList()
                .Cast<Func<MistwoodResponseEventArgs, Task>>();
                    
            await Task.WhenAll(handlers.Select(handler => handler(args)));
        }
    }

    private static bool ShouldIntercept(Uri uri) =>
        uri.Host.Contains("mistwood.pl", StringComparison.OrdinalIgnoreCase) && 
        uri.AbsolutePath.StartsWith("/api/trpc", StringComparison.OrdinalIgnoreCase);

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(MistwoodProxy));
    }
        
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        
        if (IsRunning)
        {
            if (_wasSystemProxy)
            {
                _proxyServer.DisableSystemProxy(ProxyProtocolType.AllHttp);
            }
            _proxyServer.Stop();
        }
            
        _proxyServer.Dispose();
        _disposed = true;
    }
}