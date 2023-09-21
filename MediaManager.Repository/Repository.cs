using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        /// 
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
                        valuesPlaceholder.Append($"(@CallId{i}, @RecordingId{i}, @RecorderId{i}, @StartTime{i}, @EndTime{i}, @MediaType{i}, @RecordingStatus{i}, @ArchivingStatus{i})");

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
                        recordingCmd.Parameters.AddWithValue($"@ArchivingStatus{i}", ArchivingStatus.GoingToArchive);
                    }

                    recordingCmd.CommandText += valuesPlaceholder.ToString();
                    recordingCmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Sets status archiving of recording 
        /// </summary>
        /// <param name="callEvent"></param>
        public void SetArchivingStatus(CallEvent callEvent, string archivingFilePath,ArchivingStatus archivingStatus)
        {
            using (var connection = new SqlConnection(CreateConnectionString()))
            {
                connection.Open();

                foreach (var recording in callEvent.Recordings)
                {
                    using (var recordingCmd = new SqlCommand("UPDATE Recording SET ArchivingStatus = @ArchivingStatus, ArchivingFilePath = @ArchivingFilePath, ArchivingDate = @ArchivingDate WHERE CallId = @CallId AND RecordingId = @RecordingId;", connection))
                        {
                            recordingCmd.Parameters.AddWithValue("@ArchivingStatus", archivingStatus);
                            recordingCmd.Parameters.AddWithValue("@ArchivingFilePath", archivingFilePath);
                            recordingCmd.Parameters.AddWithValue("@ArchivingDate", recording.EndTime);
                            recordingCmd.Parameters.AddWithValue("@CallId", callEvent.CallId);
                            recordingCmd.Parameters.AddWithValue("@RecordingId", recording.RecordingId);

                            recordingCmd.ExecuteNonQuery();
                        }
                }

                connection.Close();
            }

        }
    }
}
