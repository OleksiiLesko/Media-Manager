using MediaManager.RabbitMQClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MediaManager.Tests
{
    public class RabbitMQServiceTest
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<RabbitMQService>> _loggerMock;
        private readonly Mock<IModel> _channelMock;
        public RabbitMQServiceTest()
        {
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<RabbitMQService>>();
            _channelMock = new Mock<IModel>();
        }

        [Fact]
        public void Connect_ShouldCreateConnection()
        {
            _configurationMock.Setup(c => c["RabbitMQ:Host"]).Returns("localhost");

            var rabbitMQService = new RabbitMQService(_configurationMock.Object, _loggerMock.Object);

            var connection = rabbitMQService.Connect();

            Assert.NotNull(connection);
        }

        [Fact]
        public void AcknowledgeMessage_ShouldCallBasicAck()
        {
            var deliveryTag = (ulong)123;

            var rabbitMQService = new RabbitMQService(_configurationMock.Object, _loggerMock.Object);

            rabbitMQService.AcknowledgeMessage(_channelMock.Object, deliveryTag);

            _channelMock.Verify(c => c.BasicAck(deliveryTag, false), Times.Once);
        }
        [Fact]
        public void ReceiveMessage_ShouldInvokeMessageHandler()
        {
            var capturedMessage = "";
            var capturedDeliveryTag = (ulong)0;
            var messageBody = Encoding.UTF8.GetBytes("Test message");
            var eventArgs = new BasicDeliverEventArgs
            {
                DeliveryTag = 123,
                Body = new ReadOnlyMemory<byte>(messageBody)
            };


            var resetEvent = new ManualResetEventSlim(false);
            EventingBasicConsumer consumer = null;

            _channelMock.Setup(c => c.BasicConsume(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<IBasicConsumer>()))
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
                    (queue, autoAck, consumerTag, noLocal, exclusive, arguments, basicConsumer) =>
                    {
                        consumer = basicConsumer as EventingBasicConsumer;

                        Task.Run(() =>
                        {
                            consumer.HandleBasicDeliver("tag", eventArgs.DeliveryTag, false, "", "", null, eventArgs.Body);
                            resetEvent.Set();
                        });
                    });

            Action<string, ulong> messageHandler = (message, deliveryTag) =>
            {
                capturedMessage = message;
                capturedDeliveryTag = deliveryTag;
            };

            var rabbitMQService = new RabbitMQService(_configurationMock.Object, _loggerMock.Object);

            rabbitMQService.ReceiveMessage(_channelMock.Object, messageHandler);

            resetEvent.Wait(TimeSpan.FromSeconds(5));

            Assert.Equal("Test message", capturedMessage);
            Assert.Equal(123ul, capturedDeliveryTag);
        }
    }
}
