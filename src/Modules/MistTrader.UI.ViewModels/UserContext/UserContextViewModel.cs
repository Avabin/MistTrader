using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DataParsers.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MistTrader.UI.ViewModels.UserContext;

public class UserContextViewModel : ViewModel, IActivatableViewModel, IRoutableViewModel
{
    private readonly UserProfileViewModel.Factory _profileFactory;
    private readonly UserTransactionsViewModel.Factory _transactionsFactory;
    private readonly UserInventoryViewModel.Factory _inventoryFactory;
    private readonly IUserContextService _userContextService;

    [Reactive] public UserProfileViewModel? ProfileViewModel { get; set; }
    [Reactive] public UserTransactionsViewModel? TransactionsViewModel { get; set; }
    [Reactive] public UserInventoryViewModel? InventoryViewModel { get; set; }
    
    public ViewModelActivator Activator { get; } = new();
    public UserContextViewModel(UserProfileViewModel.Factory profileFactory, 
        UserTransactionsViewModel.Factory transactionsFactory,
        UserInventoryViewModel.Factory inventoryFactory,
        IUserContextService userContextService, IScreen hostScreen)
    {
        HostScreen = hostScreen;
        _profileFactory = profileFactory;
        _transactionsFactory = transactionsFactory;
        _inventoryFactory = inventoryFactory;
        _userContextService = userContextService;
        
        ViewForMixins.WhenActivated((IActivatableViewModel)this, (CompositeDisposable d) =>
        {
            InventoryViewModel ??= _inventoryFactory();
            
            var inventoryDataExtracted = _userContextService.InventoryDataExtracted;
            inventoryDataExtracted
                .Select(x => x.Inventory)
                .InvokeCommand<ImmutableList<InventoryItem>, Unit>(InventoryViewModel.MergeInventoryItemsCommand)
                .DisposeWith(d);
            
            var profileDataExtracted = _userContextService.ProfileDataExtracted;
            profileDataExtracted
                .Select(x => x.Profile)
                .Select(x => _profileFactory(x))
                .BindTo(this, vm => vm.ProfileViewModel)
                .DisposeWith(d);
            
            TransactionsViewModel ??= _transactionsFactory();
            
            var transactionDataExtracted = _userContextService.TransactionDataExtracted;
            transactionDataExtracted
                .Select(x => x.Transactions)
                .InvokeCommand<ImmutableList<Transaction>, Unit>(TransactionsViewModel.MergeTransactionsCommand)
                .DisposeWith(d);
        });
    }

    public string? UrlPathSegment { get; } = "user-context";
    public IScreen HostScreen { get; }
}