namespace MistTrader.DataExtraction.Errors;

[Flags]
public enum ExtractionError
{
    None = 0b0000000000000000,
    CannotFindInventory = 0b0001,
    CannotFindProfile = 0b0010,
    CannotFindTransactions = 0b0100,
}