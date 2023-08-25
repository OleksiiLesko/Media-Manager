namespace MediaManager.Common
{
    /// <summary>
    /// Enumeration for call event directions
    /// </summary>
    public enum CallDirection
    {
        Unknown,
        Incoming,
        Outcoming,
        Internal
    }
    /// <summary>
    /// Enumeration for call event media type 
    /// </summary>
    public enum MediaType
    {
        Voice,
        Screen
    }
    /// <summary>
    /// Enumeration for call event recording status
    /// </summary>
    public enum RecordingStatus
    {
        None, 
        Recorded, 
        NotRecorded
    }
}