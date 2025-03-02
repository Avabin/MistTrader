using DataParsers.Models;
using ReactiveUI.Fody.Helpers;

namespace MistTrader.UI.ViewModels.UserContext;

public class TransactionViewModel : ViewModel
{
    private Transaction _transaction;
    
    [Reactive] public long Id { get; set; }
    [Reactive] public long Silver { get; set; }
    [Reactive] public int Count { get; set; }
    [Reactive] public DateTime CreatedAt { get; set; }
    [Reactive] public string Maker { get; set; }
    
    public delegate TransactionViewModel Factory(Transaction transaction);
    public TransactionViewModel(Transaction transaction)
    {
        _transaction = transaction;
        
        Id = transaction.Id;
        Silver = transaction.Silver;
        Count = transaction.Count;
        CreatedAt = transaction.CreatedAt;
        Maker = transaction.Maker;
    }
}