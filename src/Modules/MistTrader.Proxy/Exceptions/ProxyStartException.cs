namespace MistTrader.Proxy.Exceptions;

public class ProxyStartException : Exception
{
    public ProxyStartException(string message, Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}