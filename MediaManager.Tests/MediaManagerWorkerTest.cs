using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using MediaManager.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;

namespace MediaManager.Tests
{
    public class MediaManagerWorkerTest
    {
        private readonly Mock<ILogger<MediaManagerWorker>> _loggerMock;
        private readonly Mock<IRabbitMQService> _rabbitMQServiceMock;
        private readonly Mock<IConnection> _rabbitConnectionMock;
        private readonly Mock<IModel> _rabbitChannelMock;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CallEvent _callEvent;

       public MediaManagerWorkerTest()
        {
            _loggerMock = new Mock<ILogger<MediaManagerWorker>>();
            _rabbitMQServiceMock = new Mock<IRabbitMQService>();
            _rabbitConnectionMock = new Mock<IConnection>();
            _rabbitChannelMock = new Mock<IModel>();
            _cancellationTokenSource = new CancellationTokenSource();
            _callEvent = new CallEvent();
        }
        [Fact]
        public async Task ExecuteAsync_Should_Handle_Message_And_Acknowledge()
        {
            var stoppingToken = _cancellationTokenSource.Token;

            _rabbitMQServiceMock.Setup(x => x.Connect()).Returns(_rabbitConnectionMock.Object);
            _rabbitConnectionMock.Setup(x => x.CreateModel()).Returns(_rabbitChannelMock.Object);

            var worker = new MediaManagerWorker(_loggerMock.Object, _rabbitMQServiceMock.Object);

            var jsonMessage = JsonConvert.SerializeObject(_callEvent);
            var messageBody = Encoding.UTF8.GetBytes(jsonMessage);

            Action<string, ulong> capturedHandler = null;
            _rabbitMQServiceMock.Setup(x => x.ReceiveMessage(It.IsAny<IModel>(), It.IsAny<Action<string, ulong>>()))
                            .Callback<IModel, Action<string, ulong>>((channel, handler) =>
                            {
                                capturedHandler = handler;
                            });

            await worker.StartProcessing(stoppingToken);

            capturedHandler?.Invoke(jsonMessage, 1);

            _rabbitMQServiceMock.Verify(x => x.AcknowledgeMessage(_rabbitChannelMock.Object, 1), Times.Once);
        }
    }
}
