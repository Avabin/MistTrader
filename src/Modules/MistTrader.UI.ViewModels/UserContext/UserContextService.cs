using MistTrader.DataExtraction.Notifications;
using ReactiveUI;

namespace MistTrader.UI.ViewModels.UserContext;

internal class UserContextService : IUserContextService
{
    private readonly IMessageBus _bus;
    
    public IObservable<InventoryDataExtracted> InventoryDataExtracted => _bus.Listen<InventoryDataExtracted>();
    public IObservable<ProfileDataExtracted> ProfileDataExtracted => _bus.Listen<ProfileDataExtracted>();
    public IObservable<TransactionDataExtracted> TransactionDataExtracted => _bus.Listen<TransactionDataExtracted>();

    public UserContextService(IMessageBus bus)
    {
        _bus = bus;
    }
}