using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MistTrader.UI.ViewModels.UserContext;
using ReactiveUI;

namespace MistTrader.UI.Views;

public partial class InventoryItemView : ReactiveUserControl<InventoryItemViewModel>
{
    public InventoryItemView()
    {
        InitializeComponent();

        this.WhenActivated(d => { });
    }
}