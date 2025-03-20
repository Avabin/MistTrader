using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using MistTrader.UI.Converters;
using ReactiveUI;
using MainViewModel = MistTrader.UI.ViewModels.Main.MainViewModel;

namespace MistTrader.UI.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        
        var foundArrowUp = this.TryFindResource("ArrowCircleUpRegularIcon", out var arrowUp);
        var foundArrowDown = this.TryFindResource("ArrowCircleDownRegularIcon", out var arrowDown);
        
        if(foundArrowUp && arrowUp is StreamGeometry up)
        {
            TradeMakerToIconConverter.ArrowCircleUpRegularIcon = up;
        }
        
        if(foundArrowDown && arrowDown is StreamGeometry down)
        {
            TradeMakerToIconConverter.ArrowCircleDownRegularIcon = down;
        }
        
        this.WhenActivated(d =>
        {
        });
    }
}
