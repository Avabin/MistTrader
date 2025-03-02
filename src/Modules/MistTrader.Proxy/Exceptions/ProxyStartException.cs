namespace MistTrader.Proxy.Exceptions;

public class ProxyStartException(string message, Exception? innerException = null) : Exception(message, innerException)
{
}