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
    public string? Body { get; }
        
    /// <summary>
    /// Timestamp of the response (UTC)
    /// </summary>
    public DateTime Timestamp { get; }
        
    public MistwoodResponseEventArgs(Uri requestUrl, int statusCode, IDictionary<string, string> headers, string? body)
    {
        RequestUrl = requestUrl;
        StatusCode = statusCode;
        Headers = headers;
        Body = body;
        Timestamp = DateTime.UtcNow;
    }
}