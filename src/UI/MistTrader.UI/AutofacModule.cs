using Autofac;
using MistTrader.UI.ViewModels;
using MistTrader.UI.Views;
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
        
        
        // Register your ViewModels
        // builder.RegisterType<YourViewModel>();
        
        // Register your Services
        // builder.RegisterType<YourService>()
        //     .As<IYourService>()
        //     .SingleInstance();
    }
}