using MediaManager.Common;

namespace MediaManager.Domain.DTOs
{
    /// <summary>
    /// Archiving rule for call direction.
    /// </summary>
    public class CallDirectionRuleConfig
    {
        public bool Enabled { get; set; }
        public int Priority { get; set; }
        public bool StopOnFailure { get; set; }
        public List<CallDirection> CallDirections { get; set; }
    }
}
