using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

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
        private readonly IArchiveManager _eventArchiver;

        /// <summary>
        /// Initializes a new instance of the MediaManagerWorker
        /// </summary>
        /// <param name="logger"></param>
        public MediaManagerWorker(ILogger<MediaManagerWorker> logger, IRabbitMQService rabbitMQService, IArchiveManager eventArchiver)
        {
            _logger = logger;
            _rabbitMQService = rabbitMQService;
            _rabbitConnection = _rabbitMQService.Connect();
            _rabbitChannel = _rabbitConnection.CreateModel();
            _eventArchiver = eventArchiver;
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
                HandleReceivedMessage(message, deliveryTag);
            };

            await Task.Run(() => _rabbitMQService.ReceiveMessage(_rabbitChannel, messageHandler));
        }
        /// <summary>
        /// Handle recieved messages from RabbitMQ
        /// </summary>
        /// <param name="message"></param>
        /// <param name="deliveryTag"></param>
        private void HandleReceivedMessage(string message, ulong deliveryTag)
        {
            CallEvent receivedCallEvent = JsonConvert.DeserializeObject<CallEvent>(message);
            _rabbitMQService.AcknowledgeMessage(_rabbitChannel, deliveryTag);
            _eventArchiver.ArchiveCallEventAsync(receivedCallEvent);
            _logger.LogInformation("Received a message: {@CallEvent}", receivedCallEvent);
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