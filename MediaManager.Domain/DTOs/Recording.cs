using MediaManager.Common;

namespace MediaManager.Domain.DTOs
{
    /// <summary>
    /// Represents a recording containing information about a recorded media file.
    /// </summary>
    public class Recording
    {
        public int RecordingId { get; set; }
        public int RecorderId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public MediaType MediaType { get; set; }
        public RecordingStatus RecordingStatus { get; set; }
        public string RecordedFilePath { get; set; }
        public DateTime ArchivingDate { get; set; }
        public string ArchivingFilePath { get; set; }
        public ArchivingStatus RecordingArchivingStatus { get; set; }
    }
}

