using MediaManager.Common;
using System.Text.Json.Serialization;

namespace MediaManager.Domain.DTOs
{
    /// <summary>
    /// Archiving rule for call duration.
    /// </summary>
    public class CallDurationRuleConfig
    {
        public bool Enabled { get; set; }
        public int Priority { get; set; }
        public bool StopOnFailure { get; set; }
        public string? ComparisonOperator { get; set; }
        public int CallDurationSeconds { get; set; }
        public ComparisonOperator ParsedComparisonOperator { get; set; }
    }
}