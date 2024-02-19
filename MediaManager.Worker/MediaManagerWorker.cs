using MediaManager.ArchivingEventManager;
using MediaManager.ArchivingRuleManager;
using MediaManager.Common;
using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using MediaManager.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text.Json;

namespace MediaManager.Worker
{
    /// <summary>
    /// Represents a background worker for managing media.
    /// </summary>
    public class MediaManagerWorker : BackgroundService
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly ILogger<MediaManagerWorker> _workerLogger;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _rabbitChannel;
        private readonly IArchivingManager _archiveEventManager;
        private readonly IRepository _repository;
        private readonly ArchivingRuleManagerService _archivingRuleManager;


        /// <summary>
        /// Initializes a new instance of the MediaManagerWorker
        /// </summary>
        /// <param name="workerLogger"></param>
        public MediaManagerWorker(ILogger<MediaManagerWorker> workerLogger,
             IRabbitMQService rabbitMQService,
             IArchivingManager archivingEventManager,
             IRepository repository,
             ArchivingRuleManagerService archivingRuleManager)
        {
            _workerLogger = workerLogger;
            _rabbitMQService = rabbitMQService;
            _rabbitConnection = _rabbitMQService.Connect();
            _rabbitChannel = _rabbitConnection.CreateModel();
            _archiveEventManager = archivingEventManager;
            _repository = repository;
            _archivingRuleManager = archivingRuleManager;
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
            try
            {
                CallEvent? receivedCallEvent = JsonConvert.DeserializeObject<CallEvent>(message);

                var filteredCall = _archivingRuleManager.ApplyRules(receivedCallEvent);
                _workerLogger.LogInformation($"Applied rules for call: CallId {filteredCall.CallId}");

                _repository.SaveCallToDatabase(filteredCall);
                _workerLogger.LogInformation($"Call saved to database: CallId {filteredCall.CallId}");

                _archiveEventManager.HandleArchivingEvent(filteredCall);
                _workerLogger.LogInformation($"Handled archiving event: CallId {filteredCall.CallId}");

                _rabbitMQService.AcknowledgeMessage(_rabbitChannel, deliveryTag);
                _workerLogger.LogInformation("Received a message: {@RuleManagedCall}", filteredCall);
            }
            catch (Exception ex)
            {
                _workerLogger.LogError("Error occurred while processing the message: " + ex.Message);
            }
        }
    }
}