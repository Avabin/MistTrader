namespace MistTrader.Proxy.Models;

public class MistwoodResponseEventArgs : EventArgs
{
    /// <summary>
    /// Original request URL
    /// </summary>
    public Uri RequestUrl { get; }
        
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; }
        
    /// <summary>
    /// Response headers
    /// </summary>
    public IDictionary<string, string> Headers { get; }
        
    /// <summary>
    /// Response body
    /// </summary>
    public ReadOnlyMemory<byte>? Body { get; }
        
    /// <summary>
    /// Timestamp of the response (UTC)
    /// </summary>
    public DateTimeOffset Timestamp { get; }
    
    /// <summary>
    /// Is the response an image
    /// </summary>
    public bool IsImage => Headers.TryGetValue("Content-Type", out var contentType) && contentType.Contains("image", StringComparison.OrdinalIgnoreCase);
        
    public MistwoodResponseEventArgs(Uri requestUrl, int statusCode, IDictionary<string, string> headers, ReadOnlyMemory<byte>? body, DateTimeOffset? timestamp = null)
    {
        RequestUrl = requestUrl;
        StatusCode = statusCode;
        Headers = headers;
        Body = body;
        Timestamp = timestamp ?? DateTimeOffset.UtcNow;
    }
}