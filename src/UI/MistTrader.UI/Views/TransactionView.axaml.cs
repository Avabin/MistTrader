using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MistTrader.UI.ViewModels.UserContext;
using ReactiveUI;

namespace MistTrader.UI.Views;

public partial class TransactionView : ReactiveUserControl<TransactionViewModel>
{
    public TransactionView()
    {
        InitializeComponent();

        this.WhenActivated(d => { });
    }
}