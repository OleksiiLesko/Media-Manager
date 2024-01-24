using MediaManager.Common;

namespace MediaManager.Domain.DTOs
{
    /// <summary>
    /// Archiving rule for call direction.
    /// </summary>
    public class CallDirectionArchivingRule
    {
        public bool Enabled { get; set; }
        public bool StopOnFailure { get; set; }
        public List<CallDirection> CallDirections { get; set; }
    }
}
