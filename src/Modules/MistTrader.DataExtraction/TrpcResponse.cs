using System.Text.Json;
using DataParsers.Models;

namespace MistTrader.DataExtraction;

internal record TrpcResponse<T> where T : class
{
    public required JsonElement Json { get; init; }
    
    public T? GetResult()
    {
        try
        {
            return FindInElement(Json);
        }
        catch
        {
            return null;
        }
    }

    private static T? FindInElement(JsonElement element)
    {
        // Try to deserialize current element
        try
        {
            var result = element.Deserialize<T>(TrpcStatics.Options);
            if (IsValidResult(result))
            {
                return result;
            }
        }
        catch
        {
            // Continue searching if deserialization fails
            
        }

        // Search in arrays
        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var result in element.EnumerateArray().Select(FindInElement).OfType<T>())
            {
                return result;
            }
        }

        // Search in objects
        if (element.ValueKind != JsonValueKind.Object) return null;
        foreach (var property in element.EnumerateObject())
        {
            if (property.Value.ValueKind is not (JsonValueKind.Object or JsonValueKind.Array)) continue;
                
            var result = FindInElement(property.Value);
            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }
    
    private static bool IsValidResult(T? result)
    {
        if (result is null) return false;
        
        return result switch
        {
            Profile profile => !Profile.IsEmpty(profile),
            InventoryItem[] items => items.All(x => !InventoryItem.IsEmpty(x)),
            Transaction[] transactions => transactions.All(x => !Transaction.IsEmpty(x)),
            _ => false
        };
    }
}
file static class TrpcStatics
{
    internal static readonly JsonSerializerOptions Options = new()
    {
        TypeInfoResolver = JsonContext.Default
    };
}
