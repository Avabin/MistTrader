namespace DataParsers.Models;

public class InventoryStatistics
{
    public int TotalCount { get; set; }
    public int ItemCount { get; set; }

    public void Calculate(IEnumerable<InventoryItem> inventoryItems)
    {
        var itemsArray = inventoryItems.ToArray();
        TotalCount = itemsArray.Sum(item => item.Count);
        ItemCount = itemsArray.Count();
    }
}
