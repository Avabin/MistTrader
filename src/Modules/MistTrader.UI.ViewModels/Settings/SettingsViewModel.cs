using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Fody.Helpers;

namespace MistTrader.UI.ViewModels.Settings;

public class SettingsViewModel : ViewModel
{
    [Reactive] public ushort ProxyPort { get; set; } = 8080;
    [Reactive] public bool RunProxyOnStartup { get; set; } = false;
}

public interface ISettingsService
{
    IObservable<ushort> ProxyPort { get; }
    Task SetProxyPort(ushort port);
}

internal class SettingsService : ISettingsService
{
    private readonly ISubject<ushort> _proxyPort = new BehaviorSubject<ushort>(8080);
    public IObservable<ushort> ProxyPort => _proxyPort.AsObservable();
    public async Task SetProxyPort(ushort port) => throw new NotImplementedException();
}