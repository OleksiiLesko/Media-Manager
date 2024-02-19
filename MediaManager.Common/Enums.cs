using System.ComponentModel;

namespace MediaManager.Common
{
    /// <summary>
    /// Types of archiving rules.
    /// </summary>
    public enum RuleType
    {
        CallDirectionArchivingRule,
        CallDurationArchivingRule,
        MediaTypeArchivingRule
    }
    /// <summary>
    ///  Enumeration comparison operators
    /// </summary>
    public enum ComparisonOperator
    {
        [Description("<")]
        LessThan,

        [Description("<=")]
        LessThanOrEqual,

        [Description("==")]
        Equal,

        [Description(">")]
        GreaterThan,

        [Description(">=")]
        GreaterThanOrEqual
    }
    /// <summary>
    /// Enumeration for call event directions
    /// </summary>
    public enum CallDirection
    {
        Unknown = 1,
        Incoming,
        Outcoming,
        Internal
    }
    /// <summary>
    /// Enumeration for call event media type 
    /// </summary>
    public enum MediaType
    {
        Voice = 1,
        Screen
    }
    /// <summary>
    /// Enumeration for call event recording status
    /// </summary>
    public enum RecordingStatus
    {
        None = 1,
        Recorded,
        NotRecorded
    }
    /// <summary>
    /// Enumeration for call event recording status
    /// </summary>
    public enum ArchivingStatus
    {
        GoingToArchive = 1,
        Archived,
        FailedToArchive,
        Deleted,
        Success,
        Partial,
        Failure,
        Unknown
    }
}