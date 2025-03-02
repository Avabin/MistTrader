using System.Reactive;
using System.Reactive.Linq;
using Commons.ReactiveCommandGenerator.Core;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MistTrader.UI.ViewModels;

public partial class MainViewModel : ViewModel
{
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