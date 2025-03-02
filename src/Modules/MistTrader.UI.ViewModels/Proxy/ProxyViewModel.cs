using System.Reactive.Disposables;
using System.Reactive.Linq;
using Commons.ReactiveCommandGenerator.Core;
using DynamicData;
using DynamicData.Binding;
using MediatR;
using MistTrader.Proxy.Commands;
using MistTrader.Proxy.Notifications;
using MistTrader.UI.ViewModels.MessageBoxes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MistTrader.UI.ViewModels.Proxy;

public partial class ProxyViewModel : ViewModel, IActivatableViewModel, IRoutableViewModel
{
    private readonly IMediator _mediator;
    private readonly IMessageBus _bus;
    private readonly IMessageBoxService _messageBoxService;

    [Reactive] public bool IsRunning { get; set; }

    [Reactive] public string Status { get; set; } = "Stopped";

    [Reactive] public int Port { get; set; } = 8080; // Default port

    [Reactive] public ulong CapturedResponsesCount { get; set; } = 0;
    
    private SourceCache<JsonResponseCaptured, DateTimeOffset> _capturedResponses = new(x => x.Timestamp);
    
    public IObservableCollection<MistwoodResponseViewModel> CapturedResponses { get; }
    
    

    public ProxyViewModel(IMediator mediator, IMessageBus bus, IMessageBoxService messageBoxService, IScreen hostScreen)
    {
        HostScreen = hostScreen;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _messageBoxService = messageBoxService;

        CapturedResponses = new ObservableCollectionExtended<MistwoodResponseViewModel>();
        
        this.WhenActivated(d =>
        {
            _capturedResponses
                .Connect()
                .Transform(x => new MistwoodResponseViewModel(x))
                .SortBy(x => x.Timestamp, sortOrder: SortDirection.Descending)
                .Bind(CapturedResponses)
                .Subscribe()
                .DisposeWith(d);


            // Register to receive proxy notifications
            bus.Listen<ProxyStarted>()
                .Subscribe(OnProxyStarted)
                .DisposeWith(d);

            bus.Listen<ProxyStopped>()
                .Subscribe(OnProxyStopped)
                .DisposeWith(d);

            bus.Listen<JsonResponseCaptured>()
                .Subscribe(OnResponseCaptured)
                .DisposeWith(d);
        });
    }

    [ReactiveCommand(canExecuteMethodName: nameof(CanStart))]
    private async Task StartProxy()
    {
        // Show a message box to confirm starting the proxy
        var result = await _messageBoxService.ShowAsync("Start Proxy", "Are you sure you want to start the proxy at port " + Port + "?", MessageBoxButton.OkCancel);
        if (result != MessageBoxResult.Ok)
            return;
        Status = "Starting...";
        await _mediator.Send(new StartProxy(Port));
    }

    [ReactiveCommand(canExecuteMethodName: nameof(CanStop))]
    private async Task StopProxy()
    {
        Status = "Stopping...";
        await _mediator.Send(new StopProxy());
    }

    private IObservable<bool> CanStart() =>
        this.WhenValueChanged(x => x.IsRunning)
            .Select(running => !running);

    private IObservable<bool> CanStop() =>
        this.WhenValueChanged(x => x.IsRunning);

    private void OnProxyStarted(ProxyStarted notification)
    {
        IsRunning = true;
        Status = $"Running on port {notification.Port}";
    }

    private void OnProxyStopped(ProxyStopped _)
    {
        IsRunning = false;
        Status = "Stopped";
    }

    private void OnResponseCaptured(JsonResponseCaptured notification)
    {
        CapturedResponsesCount++;
        
        _capturedResponses.AddOrUpdate(notification);
    }

    public ViewModelActivator Activator { get; } = new();
    public string? UrlPathSegment { get; } = "proxy";
    public IScreen HostScreen { get; }
}