using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Media.Imaging;
using Commons.ReactiveCommandGenerator.Core;
using DataParsers.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MistTrader.UI.ViewModels.UserContext;

public partial class InventoryItemViewModel : ViewModel, IActivatableViewModel
{
    private readonly InventoryItem _inventoryItem;

    [Reactive] public string ItemId { get; set; }
    [Reactive] public int Count { get; set; }

    [Reactive] public string DisplayName { get; set; }
    [Reactive] public string Slug { get; set; }
    [Reactive] public string ImagePath { get; set; }

    [Reactive] public Bitmap? Image { get; set; }

    public delegate InventoryItemViewModel Factory(InventoryItem inventoryItem);

    public InventoryItemViewModel(InventoryItem inventoryItem)
    {
        _inventoryItem = inventoryItem;

        ItemId = inventoryItem.ItemId;
        Count = inventoryItem.Count;

        DisplayName = TransformToName(ItemId);
        Slug = ToSlug(ItemId);

        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        ImagePath = Path.Combine(appDataPath, "MistTrader", "Icons", $"{Slug}.png");
        
        this.WhenActivated(disposables =>
        {
            LoadIconCommand.Execute().Subscribe().DisposeWith(disposables);
            
            this.WhenAnyValue(x => x.ImagePath)
                .Select(x => Unit.Default)
                .InvokeCommand(LoadIconCommand)
                .DisposeWith(disposables);
        });
    }

    private static string ToSlug(string itemIdOrName)
    {
        var isName = itemIdOrName.Contains(' ');
        string toConvert;
        if (isName)
        {
            toConvert = itemIdOrName;
        }
        else
        {
            toConvert = TransformToName(itemIdOrName);
        }

        return toConvert.Replace(' ', '-').ToLowerInvariant();
    }


    private static string TransformToName(string itemId)
    {
        // item id is pascal case, we need to transform it to a display name
        // on every capital letter, add a space before it
        var span = itemId.AsSpan();

        var sb = new StringBuilder();
        for (var i = 0; i < span.Length; i++)
        {
            if (char.IsUpper(span[i]) && i != 0)
            {
                sb.Append(' ');
                sb.Append(span[i]);
            }
            else
            {
                sb.Append(span[i]);
            }
        }

        return sb.ToString();
    }

    [ReactiveCommand(canExecuteMethodName: nameof(CanLoadIcon))]
    private async Task LoadIcon()
    {
        if (File.Exists(ImagePath))
        {
            await using var fs = File.OpenRead(ImagePath);
            Image = new Bitmap(fs);
        }
    }

    private IObservable<bool> CanLoadIcon()
    {
        return this.WhenAnyValue(x => x.ImagePath)
            .Select(File.Exists);
    }

    public ViewModelActivator Activator { get; } = new();
}