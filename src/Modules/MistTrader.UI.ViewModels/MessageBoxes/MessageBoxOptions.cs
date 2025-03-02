namespace MistTrader.UI.ViewModels.MessageBoxes;

public sealed record MessageBoxOptions(
    string Message,
    string Title = "",
    MessageBoxIcon Icon = MessageBoxIcon.Info,
    MessageBoxButton Buttons = MessageBoxButton.Ok);