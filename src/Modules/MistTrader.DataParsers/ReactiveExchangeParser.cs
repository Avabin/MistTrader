using System.Text.Json;

namespace DataParsers;

public class ReactiveExchangeParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    private const int BufferSize = 4096;

    public IObservable<Transaction> ParseTransactionsReactive(Stream stream)
    {
        return Observable.Create<Transaction>(observer =>
        {
            var buffer = ArrayPool<char>.Shared.Rent(BufferSize);
            var currentObject = new System.Text.StringBuilder();
            var braceCount = 0;
            var inString = false;
            var escapeNext = false;
            var parsingObject = false;
            
            try
            {
                using var reader = new StreamReader(stream);
                
                // Skip until first [
                while (!reader.EndOfStream && reader.Read() != '[') { }
                
                while (!reader.EndOfStream)
                {
                    var readCount = reader.Read(buffer, 0, BufferSize);
                    
                    for (var i = 0; i < readCount; i++)
                    {
                        var c = buffer[i];
                        
                        // Handle string content
                        if (c == '"' && !escapeNext)
                        {
                            inString = !inString;
                        }
                        escapeNext = c == '\\' && !escapeNext;

                        // Only count braces when not in a string
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

                        // Collect characters for the current object
                        if (parsingObject)
                        {
                            currentObject.Append(c);
                        }
                        
                        // When we have a complete object
                        if (parsingObject && braceCount == 0)
                        {
                            var jsonStr = currentObject.ToString();
                            if (TryParseTransaction(jsonStr.AsSpan(), out var transaction))
                            {
                                observer.OnNext(transaction);
                            }
                            
                            currentObject.Clear();
                            parsingObject = false;
                        }
                    }
                }
                
                observer.OnCompleted();
                return () => ArrayPool<char>.Shared.Return(buffer);
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
                ArrayPool<char>.Shared.Return(buffer);
                return () => { };
            }
        });
    }

    public IObservable<TransactionStats> CalculateStatsReactive(Stream stream)
    {
        var statsSubject = new BehaviorSubject<Dictionary<string, TransactionStats>>(new());
        
        ParseTransactionsReactive(stream)
            .Subscribe(
                transaction =>
                {
                    var currentStats = statsSubject.Value;
                    var itemId = transaction.SellOffer.ItemId;
                    
                    if (!currentStats.TryGetValue(itemId, out var stats))
                    {
                        stats = new TransactionStats
                        {
                            ItemId = itemId,
                            TotalCount = 0,
                            TotalVolume = 0,
                            AveragePrice = 0,
                            MinPrice = transaction.Silver,
                            MaxPrice = transaction.Silver,
                            TransactionCount = 0,
                            FirstTransaction = transaction.CreatedAt,
                            LastTransaction = transaction.CreatedAt
                        };
                    }

                    stats = stats with
                    {
                        TotalCount = stats.TotalCount + transaction.Count,
                        TotalVolume = stats.TotalVolume + (transaction.Silver * transaction.Count),
                        AveragePrice = (double)(stats.TotalVolume + (transaction.Silver * transaction.Count)) / 
                                       (stats.TotalCount + transaction.Count),
                        MinPrice = Math.Min(stats.MinPrice, transaction.Silver),
                        MaxPrice = Math.Max(stats.MaxPrice, transaction.Silver),
                        TransactionCount = stats.TransactionCount + 1,
                        LastTransaction = transaction.CreatedAt > stats.LastTransaction ? 
                            transaction.CreatedAt : stats.LastTransaction
                    };
                    
                    currentStats[itemId] = stats;
                    statsSubject.OnNext(currentStats);
                },
                error => statsSubject.OnError(error),
                () => statsSubject.OnCompleted()
            );

        return statsSubject.AsObservable();
    }

    private static bool TryParseTransaction(ReadOnlySpan<char> json, out Transaction transaction)
    {
        try
        {
#if NET7_0_OR_GREATER
            transaction = JsonSerializer.Deserialize<Transaction>(json, Options);
#else
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