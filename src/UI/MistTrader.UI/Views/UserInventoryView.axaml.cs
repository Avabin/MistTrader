using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MistTrader.UI.ViewModels.UserContext;
using ReactiveUI;

namespace MistTrader.UI.Views;

public partial class UserInventoryView : ReactiveUserControl<UserInventoryViewModel>
{
    public UserInventoryView()
    {
        InitializeComponent();

        this.WhenActivated(d => { });
    }
}