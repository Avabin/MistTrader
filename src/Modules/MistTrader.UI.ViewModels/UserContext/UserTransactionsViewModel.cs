using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Commons.ReactiveCommandGenerator.Core;
using DataParsers.Models;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace MistTrader.UI.ViewModels.UserContext;

public partial class UserTransactionsViewModel : ViewModel, IActivatableViewModel
{
    private ISourceCache<TransactionViewModel, long> _transactions = new SourceCache<TransactionViewModel, long>(x => x.Id);
    
    private readonly TransactionViewModel.Factory _transactionFactory;
    public IObservableCollection<TransactionViewModel> Transactions { get; } = new ObservableCollectionExtended<TransactionViewModel>();

    public ViewModelActivator Activator { get; } = new();
    public delegate UserTransactionsViewModel Factory();
    public UserTransactionsViewModel(TransactionViewModel.Factory transactionFactory)
    {
        _transactionFactory = transactionFactory;
        
        this.WhenActivated(disposables =>
        {
            _transactions.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(Transactions)
                .Subscribe()
                .DisposeWith(disposables);
        });
    }
    
    [ReactiveCommand]
    private void MergeTransactions(ImmutableList<Transaction> transactions)
    {
        var vms = transactions.Select(x => _transactionFactory(x));
        _transactions.Edit(innerList =>
        {
            foreach (var vm in vms)
            {
                innerList.AddOrUpdate(vm);
            }
        });
    }

}