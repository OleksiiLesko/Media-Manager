using MediaManager.Common;

namespace MediaManager.Domain.DTOs
{
    /// <summary>
    /// Represents a call event containing information about a call and its associated recordings.
    /// </summary>
    public class CallEvent
    {
        public int CallId { get; set; }
        public DateTime CallStartTime { get; set; }
        public DateTime CallEndTime { get; set; }
        public CallDirection CallDirection { get; set; }
        public List<Recording> Recordings { get; set; }
    }
}
