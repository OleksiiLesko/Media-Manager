using MediaManager.RabbitMQClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            Action<string> messageHandler = (message) =>
            {
                _logger.LogInformation("Received message : {message}", message);
            };
            _rabbitMQService.ReceiveMessage(_rabbitChannel, messageHandler);
            await Task.Delay(5000, stoppingToken);
        }
    }
}