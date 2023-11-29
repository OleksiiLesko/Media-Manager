using MediaManager.Common;
using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using MediaManager.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MediaManager.ArchivingEventManager
{
    /// <summary>
    /// Represents an archiving manager for archiving event and sent status to RabbitMQ.
    /// </summary>
    public class ArchivingManager : IArchivingManager
    {
        private readonly ILogger<ArchivingManager> _logger;
        private readonly IFileManager _fileManager;
        private readonly IRepository _repository;
        private readonly IRabbitMQService _rabbitMQService;


        /// <summary>
        /// Initializes a new instance of the MediaManagerWorker
        /// </summary>
        /// <param name="logger"></param>
        public ArchivingManager(ILogger<ArchivingManager> logger,
            IFileManager eventArchiver,
            IRepository repository,
            IRabbitMQService rabbitMQService,
            IConfiguration configuration)
        {
            _logger = logger;
            _fileManager = eventArchiver;
            _repository = repository;
            _rabbitMQService = rabbitMQService;
        }
        /// <summary>
        /// Handle archiving event for sent to RabbitMQ
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        public byte[] HandleArchivingEvent(CallEvent callEvent)
        {
            try
            {
                _fileManager.ArchiveCallEventAsync(callEvent);
                _logger.LogInformation($"CallEvent archived: CallId {callEvent.CallId}");

                var callNewStatus = GetArchivingEvent(callEvent);

                var archivingEvent = BuildArchivingEvent(callNewStatus);

                var routingKey = GetRoutingKey(callNewStatus);

                _rabbitMQService.SendMessage(routingKey, archivingEvent);
                _logger.LogInformation($"CallEvent sent to RabbitMQ: CallId {callEvent.CallId}");
                return archivingEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while getting archiving status: " + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Gets routing key 
        /// </summary>
        /// <param name="archivingEvent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string GetRoutingKey(CallArchivingEvent archivingEvent)
        {
            return $"archiving-event.{archivingEvent.CallId}.{archivingEvent.ArchivingStatus.ToString().ToLower()}";
        }
        /// <summary>
        /// Build archiving event for sent to RabbitMQ 
        /// </summary>
        /// <param name="archivingEvent"></param>
        /// <returns></returns>
        private byte[] BuildArchivingEvent(CallArchivingEvent receivedArchivingEvent)
        {
            var newArchivingEventJson = JsonConvert.SerializeObject(receivedArchivingEvent);
            return Encoding.UTF8.GetBytes(newArchivingEventJson);
        }
        /// <summary>
        /// Gets CallEvent archiving status for sent to RabbitMQ
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        private CallArchivingEvent GetArchivingEvent(CallEvent callEvent)
        {
            List<Recording> recordings = _repository.GetRecordingsArchivingStatuses(callEvent);

            ArchivingStatus archivingStatus = AnalyzeArchivingStatus(recordings);
            if (archivingStatus == ArchivingStatus.Unknown)
            {
                _logger.LogError($"Uncorrect archiving status : {archivingStatus}");
                throw new Exception($"Uncorrect archiving status : {archivingStatus}");
            }
            callEvent.ArchivingStatus = archivingStatus;
            callEvent.Recordings = recordings;

            var archivingEvent = new CallArchivingEvent(callEvent);

            return archivingEvent;
        }
        /// <summary>
        /// Returns archiving status of call event
        /// </summary>
        /// <param name="recordings"></param>
        /// <returns></returns>
        private ArchivingStatus AnalyzeArchivingStatus(List<Recording> recordings)
        {
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
                return ArchivingStatus.Partial;
            }
            else if (hasFailedToArchive)
            {
                return ArchivingStatus.Failure;
            }
            else if (hasArchived)
            {
                return ArchivingStatus.Success;
            }

            return ArchivingStatus.Unknown;
        }
    }
}
