using MediaManager.ArchivingEventManager;
using MediaManager.ArchivingRuleManager;
using MediaManager.Common;
using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using MediaManager.Repositories;
using MediaManager.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MediaManager.Tests
{
    public class MediaManagerWorkerTest
    {
        private readonly Mock<ILogger<MediaManagerWorker>> _loggerMock;
        private readonly Mock<IRabbitMQService> _rabbitMQServiceMock;
        private readonly Mock<IConnection> _rabbitConnectionMock;
        private readonly Mock<IModel> _rabbitChannelMock;
        private readonly Mock<IArchivingManager> _eventArchiverMock;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CallEvent _callEvent;
        private readonly Mock<IRepository> _repositoryMock;
        private readonly Mock<IOptionsMonitor<RulesSettings>> _rulesSettingsMock;
        private readonly Mock<ILogger<ArchivingRuleManagerService>> _loggerArchivingRuleManagerMock;
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly ArchivingRuleManagerService _archivingRuleManager;
        public MediaManagerWorkerTest()
        {
            _loggerMock = new Mock<ILogger<MediaManagerWorker>>();
            _rabbitMQServiceMock = new Mock<IRabbitMQService>();
            _rabbitConnectionMock = new Mock<IConnection>();
            _rabbitChannelMock = new Mock<IModel>();
            _eventArchiverMock = new Mock<IArchivingManager>();
            _cancellationTokenSource = new CancellationTokenSource();
            _repositoryMock = new Mock<IRepository>();
            _callEvent = new CallEvent();
            _loggerArchivingRuleManagerMock = new Mock<ILogger<ArchivingRuleManagerService>>();
            _rulesSettingsMock = new Mock<IOptionsMonitor<RulesSettings>>();
            var rulesSettingsInstance = new RulesSettings
            {
                MediaTypeArchivingRule = new MediaTypeArchivingRule
                {
                    Enabled = true,
                    StopOnFailure = false,
                    MediaTypes = new List<MediaType> { MediaType.Voice }
                },
                CallDirectionArchivingRule = new CallDirectionArchivingRule
                {
                    Enabled = true,
                    StopOnFailure = false,
                    CallDirections = new List<CallDirection> { CallDirection.Incoming }
                }
            };
            _rulesSettingsMock.Setup(x => x.CurrentValue).Returns(rulesSettingsInstance);
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _archivingRuleManager = new ArchivingRuleManagerService(
                _loggerArchivingRuleManagerMock.Object, 
                _rulesSettingsMock.Object,
                _loggerFactoryMock.Object);
        }
        [Fact]
        public async Task ExecuteAsync_Should_Recieve_Message()
        {
            var stoppingToken = _cancellationTokenSource.Token;

            _rabbitMQServiceMock.Setup(x => x.Connect()).Returns(_rabbitConnectionMock.Object);
            _rabbitConnectionMock.Setup(x => x.CreateModel()).Returns(_rabbitChannelMock.Object);

            var worker = new MediaManagerWorker(_loggerMock.Object, _rabbitMQServiceMock.Object, _eventArchiverMock.Object, _repositoryMock.Object, _archivingRuleManager);

            var jsonMessage = JsonConvert.SerializeObject(_callEvent);
            var messageBody = Encoding.UTF8.GetBytes(jsonMessage);

            Action<string, ulong> capturedHandler = null;
            _rabbitMQServiceMock.Setup(x => x.ReceiveMessage(It.IsAny<IModel>(), It.IsAny<Action<string, ulong>>()))
                            .Callback<IModel, Action<string, ulong>>((channel, handler) =>
                            {
                                capturedHandler = handler;
                            });

            await worker.StartAsync(stoppingToken);

            capturedHandler?.Invoke(jsonMessage, 1);

            _rabbitMQServiceMock.Verify(x => x.ReceiveMessage(It.IsAny<IModel>(), It.IsAny<Action<string, ulong>>()), Times.Once);
        }
    }
}