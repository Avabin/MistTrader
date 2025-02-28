using Autofac;
using MistTrader.UI.Views;
using Module = Autofac.Module;

namespace MistTrader.UI;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register your ViewModels
        builder.RegisterType<MainWindow>().SingleInstance();
        
        // Register your Views
        // builder.RegisterType<YourView>();
        
        // Register your ViewModels
        // builder.RegisterType<YourViewModel>();
        
        // Register your Services
        // builder.RegisterType<YourService>()
        //     .As<IYourService>()
        //     .SingleInstance();
    }
}