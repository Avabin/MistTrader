using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using MediatR;
using MistTrader.Proxy.Commands;
using MistTrader.Proxy.Notifications;
using MistTrader.UI.ViewModels;
using MistTrader.UI.ViewModels.Proxy;
using NSubstitute;
using ReactiveUI;

namespace MistTrader.UI.ViewModelsTests;

[TestFixture]
public class ProxyViewModelTests
{
    private IMediator _mediator = null!;
    private IMessageBus _messageBus = null!;
    private IMessageBoxService _messageBoxService = null!;
    private IScreen _hostScreen = null!;
    
    private ProxyViewModel _viewModel = null!;
    private Subject<ProxyStarted> _proxyStartedSubject = null!;
    private Subject<ProxyStopped> _proxyStoppedSubject = null!;
    private Subject<JsonResponseCaptured> _responseCapturedSubject = null!;

    [SetUp]
    public void Setup()
    {
        _mediator = Substitute.For<IMediator>();
        _messageBus = Substitute.For<IMessageBus>();
        _messageBoxService = Substitute.For<IMessageBoxService>();
        _hostScreen = Substitute.For<IScreen>();
        
        // Setup message bus subjects for testing
        _proxyStartedSubject = new Subject<ProxyStarted>();
        _proxyStoppedSubject = new Subject<ProxyStopped>();
        _responseCapturedSubject = new Subject<JsonResponseCaptured>();
        
        _messageBus.Listen<ProxyStarted>().Returns(_proxyStartedSubject);
        _messageBus.Listen<ProxyStopped>().Returns(_proxyStoppedSubject);
        _messageBus.Listen<JsonResponseCaptured>().Returns(_responseCapturedSubject);

        _viewModel = new ProxyViewModel(_mediator, _messageBus, _messageBoxService, _hostScreen);

        _viewModel.Activator.Activate();
    }

    [TearDown]
    public void Cleanup()
    {
        _proxyStartedSubject.Dispose();
        _proxyStoppedSubject.Dispose();
        _responseCapturedSubject.Dispose();
    }

    [Test]
    public void Constructor_WithNullMediator_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new ProxyViewModel(null!, _messageBus, _messageBoxService, _hostScreen);
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("mediator");
    }

    [Test] public void Constructor_WithNullBus_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new ProxyViewModel(_mediator, null!, _messageBoxService, _hostScreen);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("bus");
    }
    
    

    [Test]
    public void InitialState_ShouldHaveDefaultValues()
    {
        // Assert
        _viewModel.IsRunning.Should().BeFalse();
        _viewModel.Status.Should().Be("Stopped");
        _viewModel.Port.Should().Be(8080);
        _viewModel.CapturedResponsesCount.Should().Be(0);
        _viewModel.CapturedResponses.Should().BeEmpty();
    }

    [Test]
    public async Task StartProxy_ShouldSendStartProxyCommand()
    {
        // Arrange
        const int defaultPort = 8080;
        
        // Act
        await _viewModel.StartProxyCommand.Execute();

        // Assert
        await _mediator.Received(1).Send(Arg.Is<StartProxy>(cmd => cmd.Port == defaultPort));
        _viewModel.Status.Should().Be("Starting...");
    }

    [Test]
    public async Task StopProxy_ShouldSendStopProxyCommand()
    {
        // Arrange
        _viewModel.IsRunning = true;
        
        // Act
        await _viewModel.StopProxyCommand.Execute();

        // Assert
        await _mediator.Received(1).Send(Arg.Any<StopProxy>());
        _viewModel.Status.Should().Be("Stopping...");
    }

    [Test]
    public void OnProxyStarted_ShouldUpdateStateCorrectly()
    {
        // Arrange
        const int testPort = 9999;
        _viewModel.IsRunning = false;
        
        // Act
        _proxyStartedSubject.OnNext(new ProxyStarted(testPort));

        // Assert
        _viewModel.IsRunning.Should().BeTrue();
        _viewModel.Status.Should().Be($"Running on port {testPort}");
    }

    [Test]
    public void OnProxyStopped_ShouldUpdateStateCorrectly()
    {
        // Arrange
        _viewModel.IsRunning = true;
        _viewModel.Status = "Running on port 8080";

        // Act
        _proxyStoppedSubject.OnNext(new ProxyStopped());

        // Assert
        _viewModel.IsRunning.Should().BeFalse();
        _viewModel.Status.Should().Be("Stopped");
    }

    [Test]
    public async Task CanStart_ShouldReturnTrue_WhenNotRunning()
    {
        // Arrange
        var canExecute = false;

        // Act
        var obs = _viewModel.StartProxyCommand.CanExecute.Take(1).Do(x => canExecute = x);
        _viewModel.IsRunning = false;
        var actual = await obs;

        // Assert
        canExecute.Should().BeTrue();
        actual.Should().Be(canExecute);
    }

    [Test]
    public async Task CanStart_ShouldReturnFalse_WhenRunning()
    {
        // Arrange
        var canExecute = true;

        // Act
        var obs = _viewModel.StartProxyCommand.CanExecute.Take(1).Do(x => canExecute = x);
        _viewModel.IsRunning = true;
        var actual = await obs;

        // Assert
        canExecute.Should().BeFalse();
        actual.Should().Be(canExecute);
    }

    [Test]
    public async Task CanStop_ShouldReturnTrue_WhenRunning()
    {
        // Arrange
        _viewModel.IsRunning = true;
        var canExecute = false;

        // Act
        var obs = _viewModel.StopProxyCommand.CanExecute.Take(1).Do(x => canExecute = x);
        _viewModel.IsRunning = true;
        var actual = await obs;

        // Assert
        canExecute.Should().BeTrue();
        actual.Should().Be(canExecute);
    }

    [Test]
    public async Task CanStop_ShouldReturnFalse_WhenNotRunning()
    {
        // Arrange
        _viewModel.IsRunning = false;
        var canExecute = true;

        // Act
        var obs = _viewModel.StopProxyCommand.CanExecute.Take(1).Do(x => canExecute = x);
        _viewModel.IsRunning = false;
        var actual = await obs;

        // Assert
        canExecute.Should().BeFalse();
        actual.Should().Be(canExecute);
    }
    
    [Test]
    public async Task OnResponseCaptured_ShouldIncrementCapturedResponses()
    {
        // Arrange
        _viewModel.CapturedResponsesCount = 0;
        
        // Act
        _responseCapturedSubject.OnNext(new JsonResponseCaptured(new Uri("http://localhost"), "Test response", DateTimeOffset.Now));
        
        // Assert
        _viewModel.CapturedResponsesCount.Should().Be(1);
    }
    
    // proxy responses collection testing
    [Test]
    public async Task OnResponseCaptured_ShouldAddResponseToCapturedResponses()
    {
        // Arrange
        var response = new JsonResponseCaptured(new Uri("http://localhost"), "Test response", DateTimeOffset.Now);
        var expected = new MistwoodResponseViewModel(response);
        
        // Act
        _responseCapturedSubject.OnNext(response);
        
        // Assert
        _viewModel.CapturedResponses.Should().HaveCount(1);
        _viewModel.CapturedResponses.Should().ContainEquivalentOf(expected);
    }
    
    [Test]
    public async Task OnResponseCaptured_ShouldAddMultipleResponsesToCapturedResponses()
    {
        // Arrange
        var baseTime = DateTimeOffset.Now;
        var response1 = new JsonResponseCaptured(new Uri("http://localhost"), "Test response 1", baseTime);
        var response2 = new JsonResponseCaptured(new Uri("http://localhost"), "Test response 2", baseTime.AddSeconds(1));
        var expected1 = new MistwoodResponseViewModel(response1);
        var expected2 = new MistwoodResponseViewModel(response2);
        
        // Act
        _responseCapturedSubject.OnNext(response1);
        _responseCapturedSubject.OnNext(response2);
        
        // Assert
        _viewModel.CapturedResponses.Should().HaveCount(2);
        _viewModel.CapturedResponses.Should().ContainEquivalentOf(expected1);
        _viewModel.CapturedResponses.Should().ContainEquivalentOf(expected2);
        // should be sorted by timestamp descending
        _viewModel.CapturedResponses[0].Should().BeEquivalentTo(expected2);
        _viewModel.CapturedResponses[1].Should().BeEquivalentTo(expected1);
    }
}