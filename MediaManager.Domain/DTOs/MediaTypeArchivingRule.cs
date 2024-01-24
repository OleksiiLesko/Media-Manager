using MediaManager.Common;

namespace MediaManager.Domain.DTOs
{
    /// <summary>
    /// Archiving rule for media type.
    /// </summary>
    public class MediaTypeArchivingRule 
    {
        public bool Enabled { get; set; }
        public bool StopOnFailure { get; set; }
        public List<MediaType> MediaTypes { get; set; }
    }
}
