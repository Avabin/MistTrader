using Autofac;

namespace MistTrader.UI.ViewModels;

public class ViewModelsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MainViewModel>().SingleInstance();
    }
}