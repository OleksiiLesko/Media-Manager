using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MediaManager.RabbitMQClient
{
    /// <summary>
    /// Represents a service for interacting with RabbitMQ.
    /// </summary>
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMQService> _logger;

        /// <summary>
        /// Initializes a new instance of the RabbitMQService
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public RabbitMQService(IConfiguration configuration, ILogger<RabbitMQService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Creates and manages the RabbitMQ connection.
        /// </summary>
        /// <returns></returns>
        public IConnection Connect()
        {
            _logger.LogInformation("Starting connection service to RabbitMQ");
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:Host"] };
            var connection = factory.CreateConnection();
            _logger.LogInformation("Service connected to RabbitMQ");
            return connection;
        }

        /// <summary>
        /// Receiving messages from RabbitMQ.
        /// </summary>
        public void ReceiveMessage(IModel channel, Action<string> messageHandler)
        {
            _logger.LogInformation("Starting RabbitMQ message receive:");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var copiedMessage = message;
                messageHandler?.Invoke(copiedMessage);
                AcknowledgeMessage(channel, ea.DeliveryTag);
            };

            channel.BasicConsume(queue: _configuration["RabbitMQ:CallEventQueue"], autoAck: false, consumer: consumer);
        }
        /// <summary>
        /// Sends acknowledgment for a successfully processed message.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="deliveryTag"></param>
        private void AcknowledgeMessage(IModel channel, ulong deliveryTag)
        {
            channel.BasicAck(deliveryTag, multiple: false);
        }
    }
}
