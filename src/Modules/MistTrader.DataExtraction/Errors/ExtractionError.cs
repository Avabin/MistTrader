namespace MistTrader.DataExtraction.Errors;

[Flags]
public enum ExtractionError : ulong
{
    None = 0b0000000000000000,
    InventoryError = 0b0001,
    ProfileError = 0b0010,
    TransactionsError = 0b0100,
    MessagesError = 0b1000,
    UnreadMessagesCountError = 0b10000
}