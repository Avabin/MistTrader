using System.Buffers;
using System.Text.Json;

namespace DataParsers;

public class HighPerformanceParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public IReadOnlyList<Transaction> ParseTransactions(ReadOnlySpan<byte> utf8Json)
    {
        var reader = new Utf8JsonReader(utf8Json);
        var result = new List<Transaction>();
        
        // Skip to first array
        while (reader.Read() && reader.TokenType != JsonTokenType.StartArray) { }
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) break;
            
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var transaction = JsonSerializer.Deserialize<Transaction>(ref reader, Options);
                if (transaction != null)
                {
                    result.Add(transaction);
                }
            }
        }

        return result;
    }

    public IReadOnlyList<Transaction> ParseFile(string filePath)
    {
        var fileLength = new FileInfo(filePath).Length;
        if (fileLength == 0) return Array.Empty<Transaction>();

        // For small files use stack allocation
        if (fileLength < 16384) // 16KB
        {
            Span<byte> buffer = stackalloc byte[checked((int)fileLength)];
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            fs.Read(buffer);
            return ParseTransactions(buffer);
        }

        // For larger files use pooled buffer
        byte[] rentedBuffer = null!;
        try
        {
            rentedBuffer = ArrayPool<byte>.Shared.Rent(checked((int)fileLength));
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            var bytesRead = fs.Read(rentedBuffer, 0, checked((int)fileLength));
            return ParseTransactions(rentedBuffer.AsSpan(0, bytesRead));
        }
        finally
        {
            if (rentedBuffer != null)
            {
                ArrayPool<byte>.Shared.Return(rentedBuffer);
            }
        }
    }
}