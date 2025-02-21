namespace DataParsers.Models;

public class InventoryStatistics
{
    public long TotalValue { get; set; }
    public double AverageValue { get; set; }
    public int TotalCount { get; set; }
    public int ItemCount { get; set; }

    public void Calculate(IEnumerable<InventoryItem> inventoryItems)
    {
        TotalValue = inventoryItems.Sum(item => item.Silver * item.Count);
        TotalCount = inventoryItems.Sum(item => item.Count);
        ItemCount = inventoryItems.Count();
        AverageValue = ItemCount > 0 ? (double)TotalValue / ItemCount : 0;
    }
}
