using MediaManager.Common;
using MediaManager.Domain.DTOs;
using MediaManager.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MediaManager.RabbitMQClient
{
    /// <summary>
    /// Archiving  events to specified file paths.
    /// </summary>
    public class ArchiveManager : IArchiveManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ArchiveManager> _logger;
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the EventArchiver class.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public ArchiveManager(IConfiguration configuration, ILogger<ArchiveManager> logger, IRepository repository)
        {
            _configuration = configuration;
            _logger = logger;
            _repository = repository;
        }
        /// <summary>
        /// Copies recording file  to a specified file path.
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        public async Task ArchiveCallEventAsync(CallEvent callEvent)
        {
            var archivePath = GetArchivePath(callEvent);
            if (!Directory.Exists(archivePath))
            {
                Directory.CreateDirectory(archivePath);
            }

            foreach (var recording in callEvent.Recordings)
            {
                var sourceFilePath = recording.RecordedFilePath;
                var destinationFilePath = Path.Combine(archivePath, Path.GetFileName(sourceFilePath));
                try
                {
                     File.Copy(sourceFilePath, destinationFilePath, true);
                    _repository.SetArchivingStatus(callEvent, destinationFilePath, ArchivingStatus.Archived);
                    _logger.LogInformation("Recording copied to archive: {SourceFilePath} -> {DestinationFilePath}", sourceFilePath, destinationFilePath);
                }
                catch (Exception ex)
                {
                    _repository.SetArchivingStatus(callEvent, null, ArchivingStatus.FailedToArchive);
                    _logger.LogError("Error copying recording: {SourceFilePath} -> {DestinationFilePath}. Error: {ErrorMessage}", sourceFilePath, destinationFilePath, ex.Message);
                }
            }
        }
        /// <summary>
        /// Constructs the archive path for a given call event based on the configuration settings and call event details.
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        public string GetArchivePath(CallEvent callEvent)
        {
            var archivePathTemplate = _configuration["ArchiveEventsPath"];
            var date = callEvent.CallEndTime.ToString("dd-MM-yyyy");

            var callId = callEvent.CallId.ToString();

            return Path.Combine(archivePathTemplate.Replace("{CallEndTime}", date).Replace("{CallId}", callId));
        }
    }
}
