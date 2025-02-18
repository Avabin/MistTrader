using System.Text.Json;

namespace DataParsers;

public readonly ref struct TransactionBatch
{
    private readonly ReadOnlySpan<byte> _data;
    private readonly int _count;

    public TransactionBatch(ReadOnlySpan<byte> data, int count)
    {
        _data = data;
        _count = count;
    }

    public readonly Enumerator GetEnumerator() => new(_data);

    public ref struct Enumerator
    {
        private Utf8JsonReader _reader;
        private Transaction? _current;

        internal Enumerator(ReadOnlySpan<byte> data)
        {
            _reader = new Utf8JsonReader(data, new JsonReaderOptions 
            { 
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            });
            _current = null;

            // Skip to array start
            while (_reader.Read() && _reader.TokenType != JsonTokenType.StartArray) { }
        }

        public bool MoveNext()
        {
            while (_reader.Read())
            {
                if (_reader.TokenType == JsonTokenType.EndArray) return false;
                if (_reader.TokenType == JsonTokenType.StartObject)
                {
                    _current = JsonSerializer.Deserialize<Transaction>(ref _reader);
                    return true;
                }
            }
            return false;
        }

        public readonly Transaction Current => _current ?? throw new InvalidOperationException();
    }
}