using DataParsers.Models;
using MediatR;

namespace MistTrader.DataExtraction.Notifications;

public record ProfileDataExtracted(Profile Profile) : INotification
{
    
}