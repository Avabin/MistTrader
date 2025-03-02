using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaDialogs.Views;
using MistTrader.UI.ViewModels.MessageBoxes;
using DialogHostAvalonia;
using MistTrader.UI.ViewModels;

namespace MistTrader.UI.Services;

public class DialogHostMessageBoxService : IMessageBoxService
{
    public async Task<MessageBoxResult> ShowAsync(MessageBoxOptions options)
    {
        var dialog = CreateDialog(options);
        var result = await dialog.ShowAsync();
        return options.Buttons switch
        {
            MessageBoxButton.Ok => result switch
            {
                true => MessageBoxResult.Ok,
                false => MessageBoxResult.None,
                _ => MessageBoxResult.None,
            },
            MessageBoxButton.OkCancel => result switch
            {
                true => MessageBoxResult.Ok,
                false => MessageBoxResult.Cancel,
                _ => MessageBoxResult.None,
            },
            MessageBoxButton.YesNo => result switch
            {
                true => MessageBoxResult.Yes,
                false => MessageBoxResult.No,
                _ => MessageBoxResult.None,
            },
            MessageBoxButton.YesNoCancel => result switch
            {
                true => MessageBoxResult.Yes,
                false => MessageBoxResult.No,
                _ => MessageBoxResult.Cancel,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(options.Buttons), options.Buttons, $"Invalid {nameof(MessageBoxOptions.Buttons)} value."),
        };
    }

    public async Task<MessageBoxResult> ShowAsync(string title, string message,
        MessageBoxButton buttons = MessageBoxButton.Ok,
        MessageBoxIcon icon = MessageBoxIcon.None) =>
        await ShowAsync(new MessageBoxOptions(message, title, icon, buttons));

    private static BaseDialog CreateDialog(MessageBoxOptions options) =>
        options.Buttons switch
        {
            MessageBoxButton.Ok => new SingleActionDialog() { ButtonText = "Ok", Message = options.Message, },
            MessageBoxButton.OkCancel => new TwofoldDialog()
            {
                NegativeText = "Cancel", PositiveText = "Ok", Message = options.Message,
            },
            MessageBoxButton.YesNo => new TwofoldDialog()
            {
                NegativeText = "No", PositiveText = "Yes", Message = options.Message,
            },
            MessageBoxButton.YesNoCancel => new ThreefoldDialog()
            {
                NegativeText = "No", PositiveText = "Yes", NeutralText = "Cancel", Message = options.Message,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(options.Buttons), options.Buttons, null)
        };
}