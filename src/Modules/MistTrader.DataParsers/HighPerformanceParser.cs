using System.Buffers;
using System.IO.MemoryMappedFiles;
using System.Text.Json;

namespace DataParsers;

public class HighPerformanceParser : ITradesParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        TypeInfoResolver = JsonContext.Default,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonReaderOptions ReaderOptions = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public IReadOnlyList<Transaction> ParseTransactions(ReadOnlySpan<byte> utf8Json)
    {
        var reader = new Utf8JsonReader(utf8Json, ReaderOptions);
        var result = new List<Transaction>();
        
        while (reader.Read() && reader.TokenType != JsonTokenType.StartArray) { }
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) break;
            
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var objectStart = reader.TokenStartIndex;
                var depth = 1;
                
                while (depth > 0 && reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.StartObject) depth++;
                    else if (reader.TokenType == JsonTokenType.EndObject) depth--;
                }
                
                if (depth == 0)
                {
                    var objectLength = reader.TokenStartIndex - objectStart + 1;
                    var objectJson = utf8Json.Slice(checked((int)objectStart), checked((int)objectLength));
                    var transaction = JsonSerializer.Deserialize<Transaction>(objectJson, Options);
                    if (transaction != null)
                    {
                        result.Add(transaction);
                    }
                }
            }
        }

        return result;
    }

    public IReadOnlyList<Transaction> ParseFile(string filePath)
    {
        var fileLength = new FileInfo(filePath).Length;
        if (fileLength == 0) return [];

        if (fileLength > 1024 * 1024) // 1MB
        {
            using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
            using var accessor = mmf.CreateViewAccessor(0, fileLength, MemoryMappedFileAccess.Read);
            unsafe
            {
                byte* ptr = null;
                accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                try
                {
                    return ParseTransactions(new ReadOnlySpan<byte>(ptr, checked((int)fileLength)));
                }
                finally
                {
                    accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }
        }

        if (fileLength < 16384) // 16KB
        {
            Span<byte> buffer = stackalloc byte[checked((int)fileLength)];
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            fs.ReadExactly(buffer);
            return ParseTransactions(buffer);
        }

        byte[]? rentedBuffer = null;
        try
        {
            rentedBuffer = ArrayPool<byte>.Shared.Rent(checked((int)fileLength));
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            fs.ReadExactly(rentedBuffer, 0, checked((int)fileLength));
            return ParseTransactions(rentedBuffer.AsSpan(0, checked((int)fileLength)));
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