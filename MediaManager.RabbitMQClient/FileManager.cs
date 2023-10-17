using MediaManager.Common;
using MediaManager.Domain.DTOs;
using MediaManager.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MediaManager.RabbitMQClient
{
    /// <summary>
    /// Archiving  events to specified file paths.
    /// </summary>
    public class FileManager : IFileManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileManager> _logger;
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the EventArchiver class.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public FileManager(IConfiguration configuration, ILogger<FileManager> logger, IRepository repository)
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

            var recordingStatusesAndPaths = new List<(string?, ArchivingStatus)>();

            foreach (var recording in callEvent.Recordings)
            {
                string? destinationFilePath = null;
                ArchivingStatus recordingStatus;

                try
                {
                    destinationFilePath = Path.Combine(archivePath, Path.GetFileName(recording.RecordedFilePath));
                    File.Copy(recording.RecordedFilePath, destinationFilePath, true);
                    recordingStatus = ArchivingStatus.Archived;
                    _logger.LogInformation("Recording copied to archive: {SourceFilePath} -> {DestinationFilePath}",
                        recording.RecordedFilePath, destinationFilePath);
                }
                catch (Exception ex)
                {
                    destinationFilePath = null;
                    recordingStatus = ArchivingStatus.FailedToArchive;
                    _logger.LogError("Error copying recording: {SourceFilePath}. Error: {ErrorMessage}",
                        recording.RecordedFilePath, ex.Message);
                }

                recordingStatusesAndPaths.Add((destinationFilePath, recordingStatus));
            }
            _repository.SetCallArchivingStatusToDatabse(callEvent, recordingStatusesAndPaths);
        }

        /// <summary>
        /// Constructs the archive path for a given call event based on the configuration settings and call event details.
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        private string GetArchivePath(CallEvent callEvent)
        {
            var archivePathTemplate = _configuration["ArchiveEventsPath"];
            var date = callEvent.CallEndTime.ToString("dd-MM-yyyy");

            var callId = callEvent.CallId.ToString();

            return Path.Combine(archivePathTemplate.Replace("{CallEndTime}", date).Replace("{CallId}", callId));
        }
    }

}

