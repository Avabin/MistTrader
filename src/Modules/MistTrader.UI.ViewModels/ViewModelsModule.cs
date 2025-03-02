using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MistTrader.Proxy;

namespace MistTrader.UI.ViewModels;

public class ViewModelsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MainViewModel>().SingleInstance();
    }
}

public static class ServicesExtensions
{
    public static IServiceCollection AddViewModelsDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProxy();
        return services;
    }
}