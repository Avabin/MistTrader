using System.Web;
using MediatR;
using Microsoft.Extensions.Logging;
using MistTrader.Proxy.Notifications;
using Newtonsoft.Json;
using ReactiveUI;
using Formatting = System.Xml.Formatting;

namespace MistTrader.UI.ViewModels.Handlers;

public class ProxyResponseCapturedHandler(IMessageBus bus, ILogger<ProxyResponseCapturedHandler> logger) : INotificationHandler<JsonResponseCaptured>
{
    private readonly IMessageBus _bus = bus;
    private readonly ILogger<ProxyResponseCapturedHandler> _logger = logger;

    public Task Handle(JsonResponseCaptured notification, CancellationToken cancellationToken)
    {
        var urlWithoutQuery = notification.Url.GetLeftPart(UriPartial.Path);
        _logger.LogDebug("Response captured from {Url}", urlWithoutQuery);
        _bus.SendMessage(notification);
        return Task.CompletedTask;
    }
}

public class ProxyImageResponseCapturedHandler(IMessageBus bus, ILogger<ProxyImageResponseCapturedHandler> logger) : INotificationHandler<ImageResponseCaptured>
{
    private readonly IMessageBus _bus = bus;
    private readonly ILogger<ProxyImageResponseCapturedHandler> _logger = logger;

    public async Task Handle(ImageResponseCaptured notification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Image response captured from {Url}", notification.Url);
        // get file name from url
        var url = notification.Url;
        var fileName = Path.GetFileName(url.LocalPath);
        var query = url.Query;
        // get url query parameters
        if (!string.IsNullOrEmpty(query))
        {
            var urlDecoded = HttpUtility.UrlDecode(query);
            var queryParameters = HttpUtility.ParseQueryString(urlDecoded);
            var urlParam = queryParameters["url"] ?? "";
            var originalUrl = new Uri(urlParam);
            fileName = Path.GetFileName(originalUrl.LocalPath);
        }
        if (string.IsNullOrWhiteSpace(fileName)) return;
        // save as webp
        var baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MistTrader", "Icons");
        // create directory if it doesn't exist
        Directory.CreateDirectory(baseDir);
        
        var filePath = Path.Combine(baseDir, fileName);
        // check if file already exists
        if (File.Exists(filePath))
        {
            _logger.LogDebug("File {FileName} already exists", fileName);
            return;
        }
        
        _logger.LogDebug("Saving image {FileName} from {Url}", fileName, url);
        // save file
        await using var fs = File.Create(filePath);
        await fs.WriteAsync(notification.Response, cancellationToken);
        _logger.LogInformation("Image {FileName} saved", fileName);
        
        _bus.SendMessage(new IconDownloaded(fileName, url.ToString()));
    }
    
} 

public record IconDownloaded(string FileName, string Url) : INotification;