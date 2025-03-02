using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MistTrader.UI.ViewModels;
using ReactiveUI;

namespace MistTrader.UI;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? viewModel)
    {
        if (viewModel is null)
            return null;

        // to get registered view, we need to construct `IViewFor<T>` where T is the view model type
        var viewModelType = viewModel.GetType();
        var viewType = typeof(IViewFor<>).MakeGenericType(viewModelType);
        var view = App.ServiceProvider.GetService(viewType);
        if (view is Control control)
        {
            return control;
        }

        return new TextBlock
            { Text = "View registered in DI container for view model: " + viewModelType.Name };
    }

    public bool Match(object? data) => data is IViewModel;
}