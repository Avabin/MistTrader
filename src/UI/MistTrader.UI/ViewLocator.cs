using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MistTrader.UI.ViewModels;
using ReactiveUI;

namespace MistTrader.UI;

public class ViewLocator : IDataTemplate, IViewLocator
{
    public Control? Build(object? viewModel)
    {
        if (viewModel is null)
            return null;

        // to get registered view, we need to construct `IViewFor<T>` where T is the view model type
        var viewModelType = viewModel.GetType();
        var viewType = typeof(IViewFor<>).MakeGenericType(viewModelType);
        if (App.ServiceProvider is not { } provider) throw new InvalidOperationException("Service provider is not set");
        var view = provider.GetService(viewType);
        #if DEBUG
        Debug.WriteLine($"ViewLocator.Build: {viewModelType.Name} => {view?.GetType().Name}");
        #endif
        if (view is Control control)
        {
            control.DataContext = viewModel;
            if (view is IViewFor viewFor)
            {
                viewFor.ViewModel = viewModel;
            }
            return control;
        }

        return new TextBlock
            { Text = "View not registered in DI container for view model: " + viewModelType.Name };
    }

    public bool Match(object? data)
    {
        var match = data is ViewModel or IViewModel;
        #if DEBUG
        Debug.WriteLine($"ViewLocator.Match: {data?.GetType().Name} => {match}");
        #endif
        return match;
    }

    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
    {
        if (viewModel is null)
            return null;

        // to get registered view, we need to construct `IViewFor<T>` where T is the view model type
        var viewType = typeof(IViewFor<>).MakeGenericType(viewModel.GetType());
        if (App.ServiceProvider is not { } provider) throw new InvalidOperationException("Service provider is not set");
        var view = provider.GetService(viewType);
        #if DEBUG
        Debug.WriteLine($"ViewLocator.ResolveView: {viewModel.GetType().Name} => {view?.GetType().Name}");
        #endif
        if (view is IViewFor viewFor)
        {
            viewFor.ViewModel = viewModel;
        }
        return view as IViewFor;
    }
}