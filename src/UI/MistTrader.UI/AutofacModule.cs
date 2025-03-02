using Autofac;
using MistTrader.UI.Services;
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
        builder.RegisterType<ProxyView>().AsImplementedInterfaces().AsSelf();
        builder.RegisterType<UserContextView>().AsImplementedInterfaces().AsSelf();
        
        builder.RegisterType<UserProfileView>().AsImplementedInterfaces().AsSelf();
        builder.RegisterType<UserInventoryView>().AsImplementedInterfaces().AsSelf();
        builder.RegisterType<InventoryItemView>().AsImplementedInterfaces().AsSelf();
        
        builder.RegisterType<UserTransactionsView>().AsImplementedInterfaces().AsSelf();
        builder.RegisterType<TransactionView>().AsImplementedInterfaces().AsSelf();

        // Register your Services
        builder.RegisterType<DialogHostMessageBoxService>()
            .AsImplementedInterfaces()
            .SingleInstance();  // SingleInstance since we want one service
    }
}