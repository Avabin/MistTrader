using System.Text.Json;

namespace DataParsers;

public readonly ref struct HighPerformanceJsonParser
{
    private readonly ReadOnlySpan<byte> _json;
    private static readonly JsonReaderOptions ReaderOptions = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public HighPerformanceJsonParser(ReadOnlySpan<byte> json)
    {
        _json = json;
    }

    public TransactionEnumerator GetEnumerator() => new(_json);

    public ref struct TransactionEnumerator
    {
        private Utf8JsonReader _reader;
        private Transaction? _current;
        private readonly ReadOnlySpan<byte> _json;

        internal TransactionEnumerator(ReadOnlySpan<byte> json)
        {
            _json = json;
            _reader = new Utf8JsonReader(_json, ReaderOptions);
            _current = null;
            
            // Skip to array start
            while (_reader.Read() && _reader.TokenType != JsonTokenType.StartArray) { }
        }

        public bool MoveNext()
        {
            while (_reader.Read())
            {
                if (_reader.TokenType == JsonTokenType.EndArray) return false;
                if (_reader.TokenType != JsonTokenType.StartObject) continue;
                var objectStart = _reader.TokenStartIndex;
                var depth = 1;

                while (depth > 0 && _reader.Read())
                {
                    if (_reader.TokenType == JsonTokenType.StartObject) depth++;
                    else if (_reader.TokenType == JsonTokenType.EndObject) depth--;
                }

                if (depth != 0) continue;
                var objectLength = _reader.TokenStartIndex - objectStart + 1;
                var objectJson = _json.Slice(checked((int)objectStart), checked((int)objectLength));
                _current = JsonSerializer.Deserialize<Transaction>(objectJson);
                return true;
            }
            return false;
        }

        public readonly Transaction Current => _current ?? throw new InvalidOperationException();
    }

    public List<Transaction> ToList()
    {
        var result = new List<Transaction>();
        foreach (var transaction in this)
        {
            result.Add(transaction);
        }
        return result;
    }
}