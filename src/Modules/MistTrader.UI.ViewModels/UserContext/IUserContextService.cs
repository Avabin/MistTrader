using MistTrader.DataExtraction.Notifications;

namespace MistTrader.UI.ViewModels.UserContext;

public interface IUserContextService
{
    IObservable<InventoryDataExtracted> InventoryDataExtracted { get; }
    IObservable<ProfileDataExtracted> ProfileDataExtracted { get; }
    IObservable<TransactionDataExtracted> TransactionDataExtracted { get; }
}