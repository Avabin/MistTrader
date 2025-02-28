using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using MistTrader.UI.Views;
using ReactiveUI;
using Splat;
using Splat.Autofac;

namespace MistTrader.UI;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        // Configure Autofac
        var provider = new AutofacServiceProviderFactory();
        builder.ConfigureContainer(provider, containerBuilder =>
        {
            var locator = new AutofacDependencyResolver(containerBuilder);
            Locator.SetLocator(locator);
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();
            containerBuilder.RegisterModule<AutofacModule>();
        });
        
        // Configure services
        ConfigureServices(builder.Services);
        
        var appBuilder = BuildAvaloniaApp();

        var host = builder.Build();

        try
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            App.ServiceProvider = services;
            appBuilder.StartWithClassicDesktopLifetime(args);

            return 0;
        }
        catch (Exception ex)
        {
            // Log the exception here
            return 1;
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        
        // Register your other services here
        // services.AddScoped<IYourService, YourService>();
        // services.AddTransient<IDataService, DataService>();
    }
    
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    
    
}