using Avalonia.ReactiveUI;
using ReactiveUI;
using MainViewModel = MistTrader.UI.ViewModels.Main.MainViewModel;

namespace MistTrader.UI.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        
        this.WhenActivated(d =>
        {
        });
    }
}
