using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using DataParsers.Models;

namespace DataParsers.Parsers;

public class AsyncTradesParser : IAsyncTradesParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        TypeInfoResolver = JsonContext.Default
    };

    public async Task<IReadOnlyList<Transaction>> ParseTransactionsStreamAsync(Stream stream)
    {
        var result = new List<Transaction>();
        await foreach (var transaction in StreamTransactionsAsync(stream))
        {
            result.Add(transaction);
        }
        return result;
    }

    public async IAsyncEnumerable<Transaction> StreamTransactionsAsync(Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var jsonReader = new StreamReader(stream);
        
        // Read until we find the opening bracket
        while (!jsonReader.EndOfStream && (char)jsonReader.Read() != '[') { }
        
        var buffer = new char[4096];
        var currentObject = new StringBuilder();
        var braceCount = 0;
        var inString = false;
        var escapeNext = false;
        var parsingObject = false;
        
        while (!jsonReader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var readCount = await jsonReader.ReadAsync(buffer);
            
            for (var i = 0; i < readCount && !cancellationToken.IsCancellationRequested; i++)
            {
                var c = buffer[i];
                
                if (c == '"' && !escapeNext)
                {
                    inString = !inString;
                }
                escapeNext = c == '\\' && !escapeNext;

                if (!inString)
                {
                    if (c == '{')
                    {
                        braceCount++;
                        parsingObject = true;
                    }
                    else if (c == '}')
                    {
                        braceCount--;
                    }
                }

                if (parsingObject)
                {
                    currentObject.Append(c);
                }
                
                if (parsingObject && braceCount == 0)
                {
                    var jsonStr = currentObject.ToString();
                    var transaction = JsonSerializer.Deserialize<Transaction>(jsonStr, Options);
                    if (transaction != null)
                    {
                        yield return transaction;
                    }
                    
                    currentObject.Clear();
                    parsingObject = false;
                }
            }
        }
    }
}