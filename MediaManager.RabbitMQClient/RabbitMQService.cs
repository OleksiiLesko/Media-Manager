﻿using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data;
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
        public void ReceiveMessage(IModel channel, Action<string, ulong> messageHandler)
        {
            _logger.LogInformation("Starting RabbitMQ message receive:");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var deliveryTag = ea.DeliveryTag;
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var copiedMessage = message;
                messageHandler?.Invoke(copiedMessage, deliveryTag);
               
            };

            channel.BasicConsume(queue: _configuration["RabbitMQ:CallEventQueue"], autoAck: false, consumer: consumer);
        }
        /// <summary>
        /// Send message to RabbitMQ
        /// </summary>
        public void SendMessage(string routingKey,byte[] message)
        {
            try
            {
                using (var connection = Connect())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: _configuration["RabbitMQ:MediaEventsExchange"], type: "topic", durable: true);
                    channel.BasicPublish(exchange: _configuration["RabbitMQ:MediaEventsExchange"], routingKey: routingKey, basicProperties: null, body: message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while sending messages to RabbitMQ: " + ex.Message);
            }
        }
        /// <summary>
        /// Send acknowledgment for a successfully processed message.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="deliveryTag"></param>
        public void AcknowledgeMessage(IModel channel, ulong deliveryTag)
        {
            channel.BasicAck(deliveryTag, multiple: false);
        }

    }
}
