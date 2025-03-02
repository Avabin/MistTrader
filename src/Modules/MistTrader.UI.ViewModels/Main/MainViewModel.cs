using System.Reactive.Linq;
using Commons.ReactiveCommandGenerator.Core;
using MistTrader.UI.ViewModels.Proxy;
using ReactiveUI.Fody.Helpers;

namespace MistTrader.UI.ViewModels.Main;

public partial class MainViewModel : ViewModel
{
    private readonly Lazy<ProxyViewModel> _proxyViewModel;
    
    public ProxyViewModel ProxyViewModel => _proxyViewModel.Value;

    public MainViewModel(Lazy<ProxyViewModel> proxyViewModelFactory)
    {
        _proxyViewModel = proxyViewModelFactory;
    }
    [Reactive]
    public string Greeting { get; set; } = "";
    
    [ReactiveCommand(canExecuteMethodName: nameof(CanSayHello))]
    private void SayHello()
    {
        Greeting = "Hello, Avalonia!";
        
    }
    
    // CanExecute command must return IObservable<bool>
    private IObservable<bool> CanSayHello() => Observable.Return(Greeting == "");
}