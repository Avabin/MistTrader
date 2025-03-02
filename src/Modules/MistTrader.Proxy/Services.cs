using Microsoft.Extensions.DependencyInjection;
using MistTrader.Proxy.Commands;
using MistTrader.Proxy.Services;

namespace MistTrader.Proxy;

public static class ServicesExtensions
{
    /// <summary>
    /// Adds the Mistwood Proxy services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns></returns>
    public static IServiceCollection AddProxy(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<StartProxy>());
        services.AddSingleton<IMistwoodProxy, MistwoodProxy>();
        services.AddHostedService<MistwoodProxyCaptureEventPublisher>();
        
        return services;
    }
}