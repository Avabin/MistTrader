
using Riok.Mapperly.Abstractions;

namespace DataParsers.Models;

[Mapper]
public static partial class Mappers
{
    // From InventoryItem to AttachmentModel
    public static partial Messages.AttachmentModel ToAttachmentModel(InventoryItem inventoryItem);
    // From AttachmentModel to InventoryItem
    public static partial InventoryItem ToInventoryItem(Messages.AttachmentModel attachmentModel);
    
}