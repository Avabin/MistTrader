using Avalonia.Controls;
using Avalonia.ReactiveUI;
using MistTrader.UI.ViewModels;

namespace MistTrader.UI.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();
    }
}
