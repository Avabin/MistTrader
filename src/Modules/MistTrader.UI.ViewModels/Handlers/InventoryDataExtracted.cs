using MediatR;
using MistTrader.DataExtraction.Notifications;
using ReactiveUI;

namespace MistTrader.UI.ViewModels.Handlers;

internal class InventoryDataExtractedHandler(IMessageBus bus) : INotificationHandler<InventoryDataExtracted>
{
    private readonly IMessageBus _bus = bus;
    public Task Handle(InventoryDataExtracted notification, CancellationToken cancellationToken)
    {
        _bus.SendMessage(notification);
        return Task.CompletedTask;
    }
}

internal class ProfileDataExtractedHandler(IMessageBus bus) : INotificationHandler<ProfileDataExtracted>
{
    private readonly IMessageBus _bus = bus;
    public Task Handle(ProfileDataExtracted notification, CancellationToken cancellationToken)
    {
        _bus.SendMessage(notification);
        return Task.CompletedTask;
    }
}

internal class TransactionDataExtractedHandler(IMessageBus bus) : INotificationHandler<TransactionDataExtracted>
{
    private readonly IMessageBus _bus = bus;
    public Task Handle(TransactionDataExtracted notification, CancellationToken cancellationToken)
    {
        _bus.SendMessage(notification);
        return Task.CompletedTask;
    }
}