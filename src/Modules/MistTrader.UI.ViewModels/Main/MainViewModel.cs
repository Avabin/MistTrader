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
    public MainViewModel(Func<ProxyViewModel> proxyViewModelFactory, Func<UserContextViewModel> userContextFactory)
    {
        this.WhenActivated((CompositeDisposable d) =>
        {
            ProxyViewModel ??= proxyViewModelFactory();
            UserContextViewModel ??= userContextFactory();
            
            CanNavigateTo = true;
        });
    }
    
    private IObservable<bool> CanNavigateToView() => this.WhenAnyValue(x => x.CanNavigateTo);
    
    [ReactiveCommand(canExecuteMethodName: nameof(CanNavigateToView))]
    private async Task<IRoutableViewModel> NavigateToProxy() => await Router.Navigate.Execute(ProxyViewModel);

    [ReactiveCommand(canExecuteMethodName: nameof(CanNavigateToView))]
    private async Task<IRoutableViewModel> NavigateToUserContext() => await Router.Navigate.Execute(UserContextViewModel);
}