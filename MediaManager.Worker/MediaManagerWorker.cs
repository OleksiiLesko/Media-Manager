using MediaManager.Common;
using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using MediaManager.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

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
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the MediaManagerWorker
        /// </summary>
        /// <param name="logger"></param>
        public MediaManagerWorker(ILogger<MediaManagerWorker> logger, IRabbitMQService rabbitMQService, IArchiveManager eventArchiver, IRepository repository)
        {
            _logger = logger;
            _rabbitMQService = rabbitMQService;
            _rabbitConnection = _rabbitMQService.Connect();
            _rabbitChannel = _rabbitConnection.CreateModel();
            _eventArchiver = eventArchiver;
            _repository = repository;
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
                CallEvent receivedCallEvent = JsonConvert.DeserializeObject<CallEvent>(message);

                _repository.SaveCallToDatabase(receivedCallEvent);
                _logger.LogInformation($"CallEvent saved to database: CallId {receivedCallEvent.CallId}");

                _eventArchiver.ArchiveCallEventAsync(receivedCallEvent);
                _logger.LogInformation($"CallEvent archived: CallId {receivedCallEvent.CallId}");

                var callNewStatus = GetCallEventArchivingStatus(receivedCallEvent);

                var archivingEvent = BuildArchivingEvent(callNewStatus);

                var routingKey = GetRoutingKey(callNewStatus);

                _rabbitMQService.SendMessage(routingKey, archivingEvent);
                _logger.LogInformation($"CallEvent sent to RabbitMQ: CallId {receivedCallEvent.CallId}");

                _rabbitMQService.AcknowledgeMessage(_rabbitChannel, deliveryTag);
                _logger.LogInformation("Received a message: {@CallEvent}", receivedCallEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while processing the message: " + ex.Message);
            }
        }
        /// <summary>
        /// Gets routing key 
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string GetRoutingKey(CallEvent callEvent)
        {
            return $"archiving-event.{callEvent.CallId}.{callEvent.ArchivingStatus.ToString().ToLower()}";
        }
        /// <summary>
        /// Serializes massage of CallEvent to bytes
        /// </summary>
        /// <param name="receivedCallEvent"></param>
        /// <returns></returns>
        private byte[] BuildArchivingEvent(CallEvent receivedCallEvent)
        {
            var callInfo = new
            {
                CallId = receivedCallEvent.CallId,
                CallStartTime = receivedCallEvent.CallStartTime,
                ArchivingStatus = receivedCallEvent.ArchivingStatus.ToString()
            };

            var recordings = receivedCallEvent.Recordings.Select(recording => new
            {
                MediaType = recording.MediaType.ToString(),
                RecordingId = recording.RecordingId,
                StartTime = recording.StartTime,
                ArchivedPath = recording.ArchivingFilePath,
                RecordingArchivingStatus = recording.RecordingArchivingStatus.ToString()
            }).ToList();

            var newCallEvent = new
            {
                CallInfo = callInfo,
                Recordings = recordings
            };

            string newCallEventJson = JsonConvert.SerializeObject(newCallEvent);
            return Encoding.UTF8.GetBytes(newCallEventJson);
        }
        /// <summary>
        /// Gets CallEvent archiving status for sent to RabbitMQ
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        private CallEvent GetCallEventArchivingStatus(CallEvent callEvent)
        {
            List<Recording> recordings = _repository.GetRecordingsArchivingStatuses(callEvent);

            bool hasFailedToArchive = false;
            bool hasArchived = false;

            foreach (var recording in recordings)
            {
                if (recording.RecordingArchivingStatus == ArchivingStatus.FailedToArchive)
                {
                    hasFailedToArchive = true;
                }
                else if (recording.RecordingArchivingStatus == ArchivingStatus.Archived)
                {
                    hasArchived = true;
                }
            }

            if (hasFailedToArchive && hasArchived)
            {
                callEvent.ArchivingStatus = ArchivingStatus.Partial;
            }
            else if (hasFailedToArchive)
            {
                callEvent.ArchivingStatus = ArchivingStatus.Failure;
            }
            else if (hasArchived)
            {
                callEvent.ArchivingStatus = ArchivingStatus.Success;
            }

            callEvent.Recordings = recordings;

            return callEvent;
        }
    }
}