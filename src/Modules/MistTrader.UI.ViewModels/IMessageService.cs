using MistTrader.UI.ViewModels.MessageBoxes;

namespace MistTrader.UI.ViewModels;

public interface IMessageBoxService
{
    Task<MessageBoxResult> ShowAsync(MessageBoxOptions options);
    
    Task<MessageBoxResult> ShowAsync(string title, string message, 
        MessageBoxButton buttons = MessageBoxButton.Ok,
        MessageBoxIcon icon = MessageBoxIcon.None);
}