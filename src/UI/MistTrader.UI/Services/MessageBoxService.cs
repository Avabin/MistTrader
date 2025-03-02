using System.Threading.Tasks;
using Avalonia.Controls;
using MistTrader.UI.ViewModels;
using MistTrader.UI.ViewModels.MessageBoxes;
using MistTrader.UI.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace MistTrader.UI.Services;


public sealed class MessageBoxService : IMessageBoxService
{
    private readonly MainWindow _owner;
    
    public MessageBoxService(MainWindow owner)
    {
        _owner = owner;
    }

    public async Task<MessageBoxResult> ShowAsync(MessageBoxOptions options)
    {
        var parameters = MessageBoxMapper.ToMessageBoxParams(options);
        parameters.WindowIcon = _owner.Icon;
        
        var messageBox = MessageBoxManager.GetMessageBoxStandard(parameters);
        var result = await messageBox.ShowAsPopupAsync(_owner);
        
        return MessageBoxMapper.MapResult(result);
    }
    
    public Task<MessageBoxResult> ShowAsync(
        string title, 
        string message, 
        MessageBoxButton buttons = MessageBoxButton.Ok,
        MessageBoxIcon icon = MessageBoxIcon.None)
    {
        var options = new MessageBoxOptions(
            title,
            message,
            icon,
            buttons);
            
        return ShowAsync(options);
    }
}

public static class MessageBoxMapper
{
    public static MessageBoxStandardParams ToMessageBoxParams(MessageBoxOptions options)
    {
        var param = new MessageBoxStandardParams
        {
            ContentMessage = options.Message,
            ContentTitle = options.Title,
            ButtonDefinitions = MapButton(options.Buttons),
            Icon = MapIcon(options.Icon),
            ShowInCenter = true,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            SizeToContent = SizeToContent.WidthAndHeight
        };

        return param;
    }
    
    private static Icon MapIcon(MessageBoxIcon icon) => icon switch
    {
        MessageBoxIcon.Info => Icon.Info,
        MessageBoxIcon.Warning => Icon.Warning,
        MessageBoxIcon.Error => Icon.Error,
        MessageBoxIcon.Question => Icon.Question,
        _ => Icon.None
    };
    
    private static ButtonEnum MapButton(MessageBoxButton button) => button switch
    {
        MessageBoxButton.Ok => ButtonEnum.Ok,
        MessageBoxButton.OkCancel => ButtonEnum.OkCancel,
        MessageBoxButton.YesNo => ButtonEnum.YesNo,
        MessageBoxButton.YesNoCancel => ButtonEnum.YesNoCancel,
        _ => ButtonEnum.Ok
    };
    
    public static MessageBoxResult MapResult(ButtonResult result) => result switch
    {
        ButtonResult.Ok => MessageBoxResult.Ok,
        ButtonResult.Cancel => MessageBoxResult.Cancel,
        ButtonResult.Yes => MessageBoxResult.Yes,
        ButtonResult.No => MessageBoxResult.No,
        _ => MessageBoxResult.None
    };
}