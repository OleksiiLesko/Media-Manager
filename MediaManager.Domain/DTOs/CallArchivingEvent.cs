namespace MediaManager.Domain.DTOs
{
    /// <summary>
    /// Represents an archiving event containing information about a call and recordings.
    /// </summary>
    public class CallArchivingEvent
    {
        public int CallId { get; set; }
        public DateTime CallStartTime { get; set; }
        public string ArchivingStatus { get; set; }
        public List<RecordingArchivingEvent> RecordingArchivingEvent { get; set; }
        public CallArchivingEvent(CallEvent callEvent)
        {
            CallId = callEvent.CallId;
            CallStartTime = callEvent.CallStartTime;
            ArchivingStatus = callEvent.ArchivingStatus.ToString();

            RecordingArchivingEvent = callEvent.Recordings.Select(recording => new RecordingArchivingEvent
            {
                RecordingId = recording.RecordingId,
                StartTime = recording.StartTime,
                MediaType = recording.MediaType.ToString(),
                ArchivingFilePath = recording.ArchivingFilePath,
                RecordingArchivingStatus = recording.RecordingArchivingStatus.ToString()
            }).ToList();
        }
    }
}
