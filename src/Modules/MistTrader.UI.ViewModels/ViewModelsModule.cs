using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MistTrader.Proxy;
using ReactiveUI;

namespace MistTrader.UI.ViewModels;

public class ViewModelsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<Main.MainViewModel>().SingleInstance();
        builder.RegisterType<Proxy.ProxyViewModel>().SingleInstance();
    }
}

public static class ServicesExtensions
{
    public static IServiceCollection AddViewModelsDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<ViewModel>());
        services.AddProxy();

        services.AddSingleton(MessageBus.Current);
        return services;
    }
}