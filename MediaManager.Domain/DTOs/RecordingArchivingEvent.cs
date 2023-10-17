namespace MediaManager.Domain.DTOs
{
    public class RecordingArchivingEvent
    {
        public int RecordingId { get; set; }
        public DateTime StartTime { get; set; }
        public string MediaType { get; set; }
        public string ArchivingFilePath { get; set; }
        public string RecordingArchivingStatus { get; set; }
    }
}
