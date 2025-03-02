using System.Collections.Immutable;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Commons.ReactiveCommandGenerator.Core;
using DataParsers.Models;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using MistTrader.UI.ViewModels.Handlers;
using ReactiveUI;

namespace MistTrader.UI.ViewModels.UserContext;

public partial class UserInventoryViewModel : ViewModel, IActivatableViewModel
{
    private readonly ISourceCache<InventoryItemViewModel, string> _inventoryItems = new SourceCache<InventoryItemViewModel, string>(x => x.ItemId);
    private readonly InventoryItemViewModel.Factory _inventoryItemFactory;
    private readonly ILogger<UserInventoryViewModel> _logger;
    public IObservableCollection<InventoryItemViewModel> InventoryItems { get; } = new ObservableCollectionExtended<InventoryItemViewModel>();
    
    public delegate UserInventoryViewModel Factory();
    public ViewModelActivator Activator { get; } = new();
    public UserInventoryViewModel(InventoryItemViewModel.Factory inventoryItemFactory, ILogger<UserInventoryViewModel> logger, IMessageBus bus)
    {
        _inventoryItemFactory = inventoryItemFactory;
        _logger = logger;

        this.WhenActivated(disposables =>
        {
            var inventoryChanges = _inventoryItems.Connect()
                .ObserveOn(RxApp.TaskpoolScheduler);
            inventoryChanges
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(InventoryItems)
                .Subscribe()
                .DisposeWith(disposables);
            
            bus.Listen<IconDownloaded>()
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x => Path.GetFileNameWithoutExtension(x.FileName))
                .Select(x => _inventoryItems.Lookup(x))
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .SelectMany(x => x.LoadIconCommand.Execute())
                .Subscribe()
                .DisposeWith(disposables);
        });
    }
    
    [ReactiveCommand]
    private void MergeInventoryItems(ImmutableList<InventoryItem> inventoryItems)
    {
        _logger.LogInformation("Merging {Count} inventory items", inventoryItems.Count);
        _inventoryItems.Edit(innerList =>
        {
            foreach (var vm in inventoryItems.Select(inventoryItem => _inventoryItemFactory(inventoryItem)))
            {
                innerList.AddOrUpdate(vm);
            }
        });
    }

    [ReactiveCommand]
    private async Task LoadIcons(CancellationToken ct)
    {
        _logger.LogInformation("Downloading icons for inventory items");
        var distinctItems = InventoryItems.DistinctBy(x => x.ItemId).ToArray();

        await Parallel.ForEachAsync(distinctItems, ct, async (model, token) =>
        {
            await model.LoadIconCommand.Execute().ToTask(token);
        });
    }

}