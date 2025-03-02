using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MistTrader.UI.ViewModels.UserContext;
using ReactiveUI;

namespace MistTrader.UI.Views;

public partial class UserProfileView : ReactiveUserControl<UserProfileViewModel>
{
    public UserProfileView()
    {
        InitializeComponent();
        
        this.WhenActivated(disposables =>
        {
        });
    }
}