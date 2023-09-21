using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using MediaManager.Repositories;
using MediaManager.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;
using System.Threading;

namespace MediaManager.Tests
{
    public class MediaManagerWorkerTest
    {
        private readonly Mock<ILogger<MediaManagerWorker>> _loggerMock;
        private readonly Mock<IRabbitMQService> _rabbitMQServiceMock;
        private readonly Mock<IConnection> _rabbitConnectionMock;
        private readonly Mock<IModel> _rabbitChannelMock;
        private readonly Mock<IArchiveManager> _eventArchiverMock;
        private readonly Mock<IRepository> _repositoryMock;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CallEvent _callEvent;
        public MediaManagerWorkerTest()
        {
            _loggerMock = new Mock<ILogger<MediaManagerWorker>>();
            _rabbitMQServiceMock = new Mock<IRabbitMQService>();
            _rabbitConnectionMock = new Mock<IConnection>();
            _rabbitChannelMock = new Mock<IModel>();
            _eventArchiverMock = new Mock<IArchiveManager>();
            _repositoryMock = new Mock<IRepository>();
            _cancellationTokenSource = new CancellationTokenSource();
            _callEvent = new CallEvent();
        }
        [Fact]
        public async Task ExecuteAsync_Should_Recieve_Message()
        {
            var stoppingToken = _cancellationTokenSource.Token;

            _rabbitMQServiceMock.Setup(x => x.Connect()).Returns(_rabbitConnectionMock.Object);
            _rabbitConnectionMock.Setup(x => x.CreateModel()).Returns(_rabbitChannelMock.Object);

            var worker = new MediaManagerWorker(_loggerMock.Object, _rabbitMQServiceMock.Object, _eventArchiverMock.Object, _repositoryMock.Object);

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
