using DataParsers;
using DataParsers.Parsers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MistTrader.DataExtraction.Requests;

namespace MistTrader.DataExtraction;

public static class Services
{
    public static IServiceCollection AddDataExtraction(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<ExtractObjectsFromResponse>());
        services.AddTransient<IAsyncTradesParser, AsyncTradesParser>();
        return services;
    }
}