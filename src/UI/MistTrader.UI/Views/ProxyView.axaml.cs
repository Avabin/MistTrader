using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MistTrader.UI.ViewModels.Proxy;
using ReactiveUI;

namespace MistTrader.UI.Views;

public partial class ProxyView : ReactiveUserControl<ProxyViewModel>
{
    public ProxyView()
    {
        InitializeComponent();
        
        this.WhenActivated(disposables =>
        {
            
        });
    }
    
    
}