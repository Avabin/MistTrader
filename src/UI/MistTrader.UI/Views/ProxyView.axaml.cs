using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MistTrader.UI.ViewModels.Proxy;

namespace MistTrader.UI.Views;

public partial class ProxyView : ReactiveUserControl<ProxyViewModel>
{
    public ProxyView()
    {
        InitializeComponent();
    }
    
    
}