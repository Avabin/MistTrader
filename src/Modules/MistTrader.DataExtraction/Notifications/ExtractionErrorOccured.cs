using MediatR;
using MistTrader.DataExtraction.Errors;

namespace MistTrader.DataExtraction.Notifications;

public record ExtractionErrorOccured(string Message, ExtractionError Error) : INotification;