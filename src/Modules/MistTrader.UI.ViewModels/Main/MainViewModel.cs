using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Commons.ReactiveCommandGenerator.Core;
using MistTrader.UI.ViewModels.Proxy;
using MistTrader.UI.ViewModels.UserContext;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MistTrader.UI.ViewModels.Main;

public partial class MainViewModel : ViewModel, IScreen, IActivatableViewModel
{
    [Reactive] public ProxyViewModel ProxyViewModel { get; set; } = null!;
    [Reactive] public UserContextViewModel UserContextViewModel { get; set; } = null!;
    
    [Reactive] public bool CanNavigateTo { get; set; } = false;

    [Reactive] public RoutingState Router { get; set; } = new();
    
    public ViewModelActivator Activator { get; } = new();
    
    public delegate MainViewModel Factory();
    public MainViewModel(Lazy<ProxyViewModel> proxyVmLazy, Lazy<UserContextViewModel> userContextVmLazy)
    {
        this.WhenActivated((CompositeDisposable d) =>
        {
            ProxyViewModel ??= proxyVmLazy.Value;
            UserContextViewModel ??= userContextVmLazy.Value;
            
            CanNavigateTo = true;
        });
    }
    
    private IObservable<bool> CanNavigateToView() => this.WhenAnyValue(x => x.CanNavigateTo);
    
    [ReactiveCommand(canExecuteMethodName: nameof(CanNavigateToView))]
    private async Task<IRoutableViewModel> NavigateToProxy() => await Router.Navigate.Execute(ProxyViewModel);

    [ReactiveCommand(canExecuteMethodName: nameof(CanNavigateToView))]
    private async Task<IRoutableViewModel> NavigateToUserContext() => await Router.Navigate.Execute(UserContextViewModel);
}