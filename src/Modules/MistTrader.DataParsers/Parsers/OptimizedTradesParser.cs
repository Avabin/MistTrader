using System.Buffers;
using System.Text.Json;
using DataParsers.Models;

namespace DataParsers.Parsers;

public class OptimizedTradesParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // For small files, use a stack-allocated buffer
    private const int StackAllocThreshold = 1024 * 16; // 16KB

    public IReadOnlyList<Transaction> ParseTransactions(ReadOnlySpan<char> json)
    {
        // Skip whitespace and find opening bracket
        var start = json.IndexOf('[');
        if (start == -1) return Array.Empty<Transaction>();

        json = json[(start + 1)..];

        var result = new List<Transaction>();
        var currentDepth = 0;
        var objectStart = -1;
        var inString = false;
        var escapeNext = false;

        for (var i = 0; i < json.Length; i++)
        {
            var c = json[i];

            if (!inString)
            {
                switch (c)
                {
                    case '{':
                        currentDepth++;
                        if (currentDepth == 1) objectStart = i;
                        break;
                    case '}':
                        currentDepth--;
                        if (currentDepth == 0 && objectStart != -1)
                        {
                            var objectJson = json[objectStart..(i + 1)];
                            if (TryParseTransaction(objectJson, out var transaction))
                            {
                                result.Add(transaction);
                            }

                            objectStart = -1;
                        }

                        break;
                    case '"':
                        inString = true;
                        break;
                }
            }
            else
            {
                if (c == '"' && !escapeNext)
                {
                    inString = false;
                }

                escapeNext = c == '\\' && !escapeNext;
            }
        }

        return result;
    }

    public IReadOnlyList<Transaction> ParseFile(string filePath)
    {
        var fileInfo = new FileInfo(filePath);

        if (!fileInfo.Exists)
            return Array.Empty<Transaction>();

        if (fileInfo.Length < StackAllocThreshold)
        {
            // For small files, use stack allocation
            Span<char> buffer = stackalloc char[checked((int)fileInfo.Length)];
            using var reader = new StreamReader(filePath);
            var read = reader.Read(buffer);
            return ParseTransactions(buffer[..read]);
        }
        else
        {
            // For larger files, rent a buffer from the pool
            var buffer = ArrayPool<char>.Shared.Rent(checked((int)fileInfo.Length));
            try
            {
                using var reader = new StreamReader(filePath);
                var read = reader.Read(buffer);
                return ParseTransactions(buffer.AsSpan()[..read]);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
        }
    }

    private static bool TryParseTransaction(ReadOnlySpan<char> json, out Transaction transaction)
    {
        try
        {
#if NET7_0_OR_GREATER
            transaction = JsonSerializer.Deserialize<Transaction>(json, Options);
#else
            // For older .NET versions, we need to convert to string
            transaction = JsonSerializer.Deserialize<Transaction>(json.ToString(), Options);
#endif
            return true;
        }
        catch
        {
            transaction = default;
            return false;
        }
    }
}