namespace MediaManager.Common
{
    /// <summary>
    ///  Enumeration rules for call
    /// </summary>
    public enum Rules
    {
        MediaTypeArchivingRule
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