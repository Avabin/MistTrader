using Commons.ReactiveCommandGenerator.Core;
using DataParsers.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MistTrader.UI.ViewModels.UserContext;

public partial class TransactionStatsViewModel : ViewModel, IActivatableViewModel
{
    [Reactive] public string ItemId { get; set; }
    [Reactive] public int TotalCount { get; set; }
    [Reactive] public int TransactionCount { get; set; }
    [Reactive] public long MinPrice { get; set; }
    [Reactive] public long MaxPrice { get; set; }
    [Reactive] public long TotalVolume { get; set; }
    [Reactive] public double AveragePrice { get; set; }
    [Reactive] public DateTime FirstTransaction { get; set; }
    [Reactive] public DateTime LastTransaction { get; set; }
    [Reactive] public double MedianPrice { get; set; }
    [Reactive] public double StandardDeviation { get; set; }
    
    [Reactive] public long TotalBought { get; set; }
    [Reactive] public long TotalSpent { get; set; }
    [Reactive] public long TotalSold { get; set; }
    [Reactive] public long TotalEarned { get; set; }
    [Reactive] public long ProfitLoss { get; set; }
    public ViewModelActivator Activator { get; } = new();
    
    public delegate TransactionStatsViewModel Factory(TransactionStats transactionStats);

    public TransactionStatsViewModel(TransactionStats transactionStats)
    {
        ApplyStats(transactionStats);
    }

    [ReactiveCommand]
    private void ApplyStats(TransactionStats stats)
    {
        ItemId = stats.ItemId;
        TotalCount = stats.TotalCount;
        TransactionCount = stats.TransactionCount;
        MinPrice = stats.MinPrice;
        MaxPrice = stats.MaxPrice;
        TotalVolume = stats.TotalVolume;
        AveragePrice = stats.AveragePrice;
        FirstTransaction = stats.FirstTransaction;
        LastTransaction = stats.LastTransaction;
        MedianPrice = stats.MedianPrice;
        StandardDeviation = stats.StandardDeviation;
        TotalBought = stats.TotalBought;
        TotalSpent = stats.TotalSpent;
        TotalSold = stats.TotalSold;
        TotalEarned = stats.TotalEarned;
        ProfitLoss = stats.ProfitLoss;
    }
}