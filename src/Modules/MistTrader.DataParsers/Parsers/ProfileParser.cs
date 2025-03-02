using System.Text.Json;
using DataParsers.Models;

namespace DataParsers.Parsers;

public class ProfileParser
{
    public static Profile? ParseJson(JsonElement element)
    {
        // check if starts with `result` element
        var isResult = element.TryGetProperty("result", out var resultElement);
        // if so, extract data.json element
        var dataElement = isResult ? resultElement.GetProperty("data").GetProperty("json") : element;
        
        var result = JsonSerializer.Deserialize<Profile?>(dataElement.GetRawText(), JsonContext.Default.Options);
        
        return result;
    }
}