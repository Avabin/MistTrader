using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Logging;
using MistTrader.UI.ViewModels;
using ReactiveUI;
using Splat;
using Splat.Autofac;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace MistTrader.UI;

class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);
        
        // Configure Autofac
        builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.ConfigureContainer(static (HostBuilderContext context, ContainerBuilder containerBuilder) =>
        {
            // Create and configure AutofacDependencyResolver using the extension method
            var resolver = containerBuilder.UseAutofacDependencyResolver();
            
            // Register the resolver itself so it can be resolved later if needed
            containerBuilder.RegisterInstance(resolver);
            
            // Initialize ReactiveUI with the resolver
            resolver.InitializeReactiveUI();
            
            // Register Avalonia-specific services
            containerBuilder.Register(_ => new AvaloniaActivationForViewFetcher())
                .As<IActivationForViewFetcher>()
                .SingleInstance();
                
            containerBuilder.Register(_ => new AutoDataTemplateBindingHook())
                .As<IPropertyBindingHook>()
                .SingleInstance();
                
            // Set up ReactiveUI scheduler
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
            
            // Register your application services
            containerBuilder.RegisterModule<AutofacModule>();
        });

        builder.ConfigureServices(ConfigureAppServices);
        
        builder.ConfigureLogging(c => c.SetMinimumLevel(LogLevel.Debug));

        var appBuilder = BuildAvaloniaApp();
        var host = builder.Build();
        host.Start();

        try
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            App.ServiceProvider = services;
            var code = appBuilder.StartWithClassicDesktopLifetime(args);

            _ = host.StopAsync();
            host.WaitForShutdown();
            return code;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 1;
        }
    }

    private static void ConfigureAppServices(HostBuilderContext ctx, IServiceCollection services)
    {
        services.AddViewModelsDependencies(ctx.Configuration);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}