namespace DataParsers;

public interface ITradesParser
{
    IReadOnlyList<Transaction> ParseTransactions(ReadOnlySpan<byte> utf8Json);
    IReadOnlyList<Transaction> ParseFile(string filePath);
}