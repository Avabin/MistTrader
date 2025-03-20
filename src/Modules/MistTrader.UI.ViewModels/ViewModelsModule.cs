using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MistTrader.DataExtraction;
using MistTrader.Proxy;
using MistTrader.UI.ViewModels.Main;
using MistTrader.UI.ViewModels.Proxy;
using MistTrader.UI.ViewModels.UserContext;
using ReactiveUI;

namespace MistTrader.UI.ViewModels;

public class ViewModelsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MainViewModel>().SingleInstance().AsSelf().As<IScreen>();
        builder.RegisterType<ProxyViewModel>().SingleInstance();

        builder.RegisterType<UserContextService>().AsImplementedInterfaces().SingleInstance();
        
        builder.RegisterType<UserContextViewModel>().SingleInstance();
        builder.RegisterType<UserProfileViewModel>();
        builder.RegisterType<UserInventoryViewModel>();
        builder.RegisterType<InventoryItemViewModel>();
        builder.RegisterType<UserTransactionsViewModel>();
        builder.RegisterType<TransactionViewModel>();
        builder.RegisterType<TransactionStatsViewModel>();
        
    }
}

public static class ServicesExtensions
{
    public static IServiceCollection AddViewModelsDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<ViewModel>());
        services.AddProxy();
        services.AddDataExtraction();
        services.AddHttpClient();

        services.AddSingleton(MessageBus.Current);
        return services;
    }
}