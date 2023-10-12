using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace MediaManager.Repositories
{
    /// <summary>
    /// Configuration for work with database
    /// </summary>
    public class Repository : IRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Repository> _logger;
       /// <summary>
       /// Initializes a new instance of the DbConfiguration
       /// </summary>
       /// <param name="configuration"></param>
       public Repository(IConfiguration configuration, ILogger<Repository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        /// <summary>
        /// Creates connection to database
        /// </summary>
        /// <returns></returns>
        private string CreateConnectionString()
        {
            string server = _configuration["DbHostName:Server"];
            string database = _configuration["DbHostName:Database"];

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                TrustServerCertificate = true,
                IntegratedSecurity = true,
                ConnectRetryCount = 1
            };

            return builder.ConnectionString;
        }
        /// <summary>
        /// Works with database
        /// </summary>
        public void SaveCallToDatabase(CallEvent callEvent)
        {
            using (var connection = new SqlConnection(CreateConnectionString()))
            {
                connection.Open();
                using (var callEventCmd = new SqlCommand("INSERT INTO CallEvent (CallId, CallStartTime, CallEndTime, CallDirection) VALUES (@CallId, @StartTime, @EndTime, @CallDirection);", connection))
                {
                    callEventCmd.Parameters.AddWithValue("@CallId", callEvent.CallId);
                    callEventCmd.Parameters.AddWithValue("@StartTime", callEvent.CallStartTime);
                    callEventCmd.Parameters.AddWithValue("@EndTime", callEvent.CallEndTime);
                    callEventCmd.Parameters.AddWithValue("@CallDirection", callEvent.CallDirection);

                    callEventCmd.ExecuteNonQuery();
                }

                using (var recordingCmd = new SqlCommand("INSERT INTO Recording (CallId,RecordingId,RecorderId,StartTime,EndTime,MediaType,RecordingStatus,ArchivingStatus) VALUES ", connection))
                {
                    var valuesPlaceholder = new StringBuilder();

                    for (int i = 0; i < callEvent.Recordings.Count; i++)
                    {
                        var recording = callEvent.Recordings[i];
                        valuesPlaceholder.Append($"(@CallId{i}, @RecordingId{i}, @RecorderId{i}, @StartTime{i}, @EndTime{i}, @MediaType{i}, @RecordingStatus{i}, @RecordingArchivingStatus{i})");

                        if (i < callEvent.Recordings.Count - 1)
                        {
                            valuesPlaceholder.Append(", ");
                        }

                        recordingCmd.Parameters.AddWithValue($"@CallId{i}", callEvent.CallId);
                        recordingCmd.Parameters.AddWithValue($"@RecordingId{i}", recording.RecordingId);
                        recordingCmd.Parameters.AddWithValue($"@RecorderId{i}", recording.RecorderId);
                        recordingCmd.Parameters.AddWithValue($"@StartTime{i}", recording.StartTime);
                        recordingCmd.Parameters.AddWithValue($"@EndTime{i}", recording.EndTime);
                        recordingCmd.Parameters.AddWithValue($"@MediaType{i}", recording.MediaType);
                        recordingCmd.Parameters.AddWithValue($"@RecordingStatus{i}", recording.RecordingStatus);
                        recordingCmd.Parameters.AddWithValue($"@RecordingArchivingStatus{i}", ArchivingStatus.GoingToArchive);
                    }

                    recordingCmd.CommandText += valuesPlaceholder.ToString();
                    recordingCmd.ExecuteNonQuery();
                    _logger.LogInformation($"Saved call event CallId {callEvent.CallId} to the database.");
                }
                connection.Close();
            }
        }
        /// <summary>
        /// Sets status archiving of recording 
        /// </summary>
        /// <param name="callEvent"></param>
        public void SetCallArchivingStatusToDatabse(CallEvent callEvent, List<(string?, ArchivingStatus)> recordingStatusesAndPaths)
        {
            using (var connection = new SqlConnection(CreateConnectionString()))
            {
                connection.Open();

                for (int i = 0; i < recordingStatusesAndPaths.Count; i++)
                {
                    var (path, status) = recordingStatusesAndPaths[i];
                    var recording = callEvent.Recordings[i];

                    using (var recordingCmd = new SqlCommand("UPDATE Recording SET ArchivingStatus = @RecordingArchivingStatus, ArchivingFilePath = @ArchivingFilePath, ArchivingDate = @ArchivingDate WHERE CallId = @CallId AND RecordingId = @RecordingId;", connection))
                    {
                        recordingCmd.Parameters.AddWithValue("@ArchivingDate", recording.EndTime);
                        recordingCmd.Parameters.AddWithValue("@ArchivingFilePath", path != null ? path : DBNull.Value);
                        recordingCmd.Parameters.AddWithValue("@RecordingArchivingStatus", status);
                        recordingCmd.Parameters.AddWithValue("@CallId", callEvent.CallId);
                        recordingCmd.Parameters.AddWithValue("@RecordingId", recording.RecordingId);

                        recordingCmd.ExecuteNonQuery();
                        _logger.LogInformation($"Updated call event archiving status of recording RecordingId {recording.RecordingId} in the database.");
                    }
                }

                connection.Close();
            }
        }

        /// <summary>
        /// Gets archiving status of recordings
        /// </summary>
        /// <param name="callId"></param>
        /// <returns></returns>
        public List<Recording> GetRecordingsArchivingStatuses(CallEvent callEvent)
        {
            using (var connection = new SqlConnection(CreateConnectionString()))
            {
                connection.Open();

                List<Recording> uniqueRecordings = new List<Recording>();

                using (var cmd = new SqlCommand("SELECT RecordingId, StartTime, MediaType, ArchivingFilePath, ArchivingStatus FROM Recording WHERE CallId = @CallId;", connection))
                {
                    cmd.Parameters.AddWithValue("@CallId", callEvent.CallId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var recording = new Recording
                            {
                                RecordingId = (int)reader["RecordingId"],
                                StartTime = (DateTime)reader["StartTime"],
                                MediaType = (MediaType)Enum.Parse(typeof(MediaType), reader["MediaType"].ToString()),
                                ArchivingFilePath = reader["ArchivingFilePath"].ToString(),
                                RecordingArchivingStatus = (ArchivingStatus)Enum.Parse(typeof(ArchivingStatus), reader["ArchivingStatus"].ToString())
                            };

                            uniqueRecordings.Add(recording);
                        }
                    }
                }

                return uniqueRecordings;
            }
        }
    }
}
