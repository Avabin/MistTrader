using System.Text.Json;
using DataParsers.Models;
using DataParsers.Models.Messages;

namespace DataParsers.Parsers;

public static class MessagesParser
{
    public static MessagesData? ParseMessages(JsonElement rootElement)
    {
        var jsonData = rootElement.GetProperty("result").GetProperty("data").GetProperty("json");
        return jsonData.Deserialize<MessagesData>(JsonContext.Default.Options);
    }
}