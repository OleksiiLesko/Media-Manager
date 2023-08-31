using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;
using System;

namespace MediaManager.Worker
{
    /// <summary>
    /// Represents a background worker for managing media.
    /// </summary>
    public class MediaManagerWorker : BackgroundService
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly ILogger<MediaManagerWorker> _logger;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _rabbitChannel;
        /// <summary>
        /// Initializes a new instance of the MediaManagerWorker
        /// </summary>
        /// <param name="logger"></param>
        public MediaManagerWorker(ILogger<MediaManagerWorker> logger, IRabbitMQService rabbitMQService)
        {
            _logger = logger;
            _rabbitMQService = rabbitMQService;
            _rabbitConnection = _rabbitMQService.Connect();
            _rabbitChannel = _rabbitConnection.CreateModel();
        }
        /// <summary>
        /// Executes the media management task asynchronously.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Action<string, ulong> messageHandler = (message, deliveryTag) =>
            {
                CallEvent receivedCallEvent = JsonConvert.DeserializeObject<CallEvent>(message);

                _rabbitMQService.AcknowledgeMessage(_rabbitChannel, deliveryTag);
                _logger.LogInformation("Received a message: {@CallEvent}", receivedCallEvent);
            };

            await Task.Run(() => _rabbitMQService.ReceiveMessage(_rabbitChannel, messageHandler));
        }
        /// <summary>
        /// Calls ExecuteAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartProcessing(CancellationToken cancellationToken)
        {
            await ExecuteAsync(cancellationToken);
        }
    }
}