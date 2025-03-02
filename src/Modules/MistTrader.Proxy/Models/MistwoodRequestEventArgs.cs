namespace MistTrader.Proxy.Models;

public class MistwoodRequestEventArgs : EventArgs
{
    /// <summary>
    /// Full URL of the request
    /// </summary>
    public Uri Url { get; }
        
    /// <summary>
    /// HTTP method (GET, POST, etc.)
    /// </summary>
    public string Method { get; }
        
    /// <summary>
    /// Request headers
    /// </summary>
    public IDictionary<string, string> Headers { get; }
        
    /// <summary>
    /// Request body (if any)
    /// </summary>
    public ReadOnlyMemory<byte>? Body { get; }
        
    /// <summary>
    /// Timestamp of the request (UTC)
    /// </summary>
    public DateTime Timestamp { get; }
        
    public MistwoodRequestEventArgs(Uri url, string method, IDictionary<string, string> headers, ReadOnlyMemory<byte>? body)
    {
        Url = url;
        Method = method;
        Headers = headers;
        Body = body;
        Timestamp = DateTime.UtcNow;
    }
}