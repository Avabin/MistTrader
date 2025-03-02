using System.Collections.Generic;
using Autofac;
using Avalonia.Media;
using MistTrader.UI.Services;
using MistTrader.UI.ViewModels;
using MistTrader.UI.Views;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.ViewModels;
using Module = Autofac.Module;

namespace MistTrader.UI;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register your ViewModels
        builder.RegisterModule<ViewModelsModule>();
        
        // Register your Views
        builder.RegisterType<MainWindow>().SingleInstance().AsImplementedInterfaces().AsSelf();
        builder.RegisterType<ProxyView>().SingleInstance().AsImplementedInterfaces().AsSelf();

        // Register your Services
        builder.RegisterType<MessageBoxService>()
            .AsImplementedInterfaces()
            .SingleInstance();  // SingleInstance since we want one service
    }
}