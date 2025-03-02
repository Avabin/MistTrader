namespace MistTrader.Proxy.Notifications;

public enum ProxyErrorType
{
    Unknown,
    CertificateInitializationFailed,
    ProxyAlreadyRunning,
    ProxyNotRunning,
    ProxyFailedToStart,
    ProxyFailedToStop,
    ProxyFailedToCaptureResponse
}