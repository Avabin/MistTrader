using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Commons.ReactiveCommandGenerator.Core;
using DataParsers;
using DataParsers.Models;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using ReactiveUI;

namespace MistTrader.UI.ViewModels.UserContext;

public partial class UserTransactionsViewModel : ViewModel, IActivatableViewModel
{
    private ISourceCache<TransactionViewModel, long> _transactions = new SourceCache<TransactionViewModel, long>(x => x.Id);
    private ISourceCache<TransactionStatsViewModel, string> _stats = new SourceCache<TransactionStatsViewModel, string>(x => x.ItemId);
    
    private readonly TransactionViewModel.Factory _transactionFactory;
    private readonly TransactionStatsViewModel.Factory _statsVmFactory;
    public IObservableCollection<TransactionViewModel> Transactions { get; } = new ObservableCollectionExtended<TransactionViewModel>();
    public IObservableCollection<TransactionStatsViewModel> Stats { get; } = new ObservableCollectionExtended<TransactionStatsViewModel>();

    public ViewModelActivator Activator { get; } = new();
    public delegate UserTransactionsViewModel Factory();
    public UserTransactionsViewModel(TransactionViewModel.Factory transactionFactory, TransactionStatsViewModel.Factory statsVmFactory)
    {
        _transactionFactory = transactionFactory;
        _statsVmFactory = statsVmFactory;

        this.WhenActivated(disposables =>
        {
            var transactionsObs = _transactions.Connect();
            transactionsObs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(Transactions)
                .Subscribe()
                .DisposeWith(disposables);
            
            var statsObs = _stats.Connect();
            statsObs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(Stats)
                .Subscribe()
                .DisposeWith(disposables);
        });
    }
    
    [ReactiveCommand]
    private void MergeTransactions(IReadOnlyList<Transaction> transactions)
    {
        var vms = transactions.Select(x => _transactionFactory(x));
        _transactions.Edit(innerList =>
        {
            foreach (var vm in vms)
            {
                innerList.AddOrUpdate(vm);
            }
        });
        
        // edit stats
        var grouped = transactions.GroupBy(t => t.SellOffer.ItemId).ToArray();
        if (grouped.Length == 0)
        {
            return;
        }
        _stats.Edit(innerList =>
        {
            var existingGroups = innerList.Items.ToImmutableDictionary(x => x.ItemId, x => x);
            
            // if the stats for a given item already exist, update them
            foreach (var grouping in grouped)
            {
                if (existingGroups.TryGetValue(grouping.Key, out var existingStats))
                {
                    var stats = TradeStatsCalculator.CalculateStat(grouping.ToList());
                    existingStats.ApplyStatsCommand.Execute(stats);
                }
                else
                {
                    var stats = TradeStatsCalculator.CalculateStat(grouping.ToList());
                    var vm = _statsVmFactory(stats);
                    innerList.AddOrUpdate(vm);
                }
            }
        });
    }

}