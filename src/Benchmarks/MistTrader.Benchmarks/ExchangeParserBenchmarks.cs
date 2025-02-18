using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DataParsers;

namespace MistTrader.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class ExchangeParserBenchmarks
{
    private string _sampleJson = string.Empty;
    private byte[] _jsonBytes = [];
    private Stream _jsonStream = null!;
    private IAsyncTradesParser _asyncParser = null!;
    private ITradesParser _highPerformanceParser = null!;
    private IReactiveTradesParser _reactiveParser = null!;
    
    private static readonly DateTime BaseTime = DateTime.Parse("2025-02-17 22:47:01");

    private string _tempFile = string.Empty;

    [Params(10, 100, 1000)] 
    public int TransactionCount { get; set; }

    private Stream CreateJsonStream() => new MemoryStream(Encoding.UTF8.GetBytes(_sampleJson));

    [GlobalSetup]
    [RequiresUnreferencedCode(
        "The ExchangeFileParser types use reflection and may require dynamic code generation.")]
    [RequiresDynamicCode(
        "The ExchangeFileParser types use reflection and may require dynamic code generation.")]
    public void Setup()
    {
        _asyncParser = new ExchangeFileParser();
        _highPerformanceParser = new HighPerformanceParser();
        _reactiveParser = new ReactiveTradesParser();

        var transactions = GenerateTransactions(TransactionCount);
        _sampleJson = JsonSerializer.Serialize(transactions);
        _jsonBytes = Encoding.UTF8.GetBytes(_sampleJson);

        _jsonStream = CreateJsonStream();

        _tempFile = Path.GetTempFileName();
        File.WriteAllText(_tempFile, _sampleJson);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _jsonStream.Dispose();
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
    }

    [Benchmark(Baseline = true)]
    public async Task<IReadOnlyList<Transaction>> ParseFullFile()
    {
        _jsonStream.Position = 0;
        return await _asyncParser.ParseTransactionsStreamAsync(_jsonStream);
    }

    private volatile int _counter = 0;
    [Benchmark]
    public async Task StreamTransactions()
    {
        _jsonStream.Position = 0;
        await foreach (var _ in _asyncParser.StreamTransactionsAsync(_jsonStream))
        {
            _counter++;
        }
    }

    private volatile int _secondCounter = 0;
    [Benchmark]
    public async Task StreamTransactionsWithPreallocation()
    {
        _jsonStream.Position = 0;
        await foreach (var _ in _asyncParser.StreamTransactionsAsync(_jsonStream))
        {
            _secondCounter++;
        }
    }

    [Benchmark]
    public IReadOnlyList<Transaction> ParseHighPerformance()
    {
        return _highPerformanceParser.ParseTransactions(new ReadOnlySpan<byte>(_jsonBytes));
    }

    [Benchmark]
    public IReadOnlyList<Transaction> ParseHighPerformanceFromFile()
    {
        return _highPerformanceParser.ParseFile(_tempFile);
    }

    // parse using HighPerformanceJsonParser
    [Benchmark]
    public int ParseHighPerformanceJson()
    {
        var parser = new HighPerformanceJsonParser(_jsonBytes);
        var count = 0;
        foreach (var transaction in parser)
        {
            count++;
        }
        
        return count;
    }

    [Benchmark]
    public async Task ParseReactive()
    {
        using var stream = CreateJsonStream();
        var count = 0;
        await _reactiveParser.ParseTransactionsReactive(stream)
            .Do(_ => count++)
            .LastOrDefaultAsync();
    }

    [Benchmark]
    public async Task ParseReactiveWithStats()
    {
        using var stream = CreateJsonStream();
        await _reactiveParser.CalculateStatsReactive(stream)
            .LastOrDefaultAsync();
    }

    private static IEnumerable<Transaction> GenerateTransactions(int count)
    {
        var rng = new Random(42);
        var now = DateTime.UtcNow;

        for (var i = 0; i < count; i++)
        {
            var silver = rng.NextInt64(100_000, 1_000_000);
            var itemCount = rng.Next(1, 100);
            
            yield return new Transaction
            {
                Id = i,
                Maker = $"Maker_{i}",
                CreatedAt = now.AddMinutes(-i),
                Count = itemCount,
                Silver = silver,
                SellOffer = new BreederOffer
                {
                    Id = i * 2,
                    BreederId = i * 3,
                    ItemId = $"Item_{i % 3}", // Create some duplicate items
                    Silver = silver
                },
                BuyOffer = new BreederOffer
                {
                    Id = i * 2 + 1,
                    BreederId = i * 3 + 1,
                    ItemId = $"Item_{i % 3}",
                    Silver = silver
                }
            };
        }
    }
}