using MistTrader.Proxy.Notifications;
using ReactiveUI.Fody.Helpers;

namespace MistTrader.UI.ViewModels.Proxy;

public class MistwoodResponseViewModel : ViewModel
{
    [Reactive] public Uri Url { get; set; }
    [Reactive] public DateTimeOffset Timestamp { get; set; }
    
    [Reactive] public ulong ResponseLength { get; set; }

    public MistwoodResponseViewModel(JsonResponseCaptured captured)
    {
        Url = captured.Url;
        Timestamp = captured.Timestamp;
        ResponseLength = (ulong)captured.Response.Length;
    }
}