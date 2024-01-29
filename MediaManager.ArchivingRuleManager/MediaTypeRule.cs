using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;

namespace MediaManager.ArchivingRuleManager
{
    /// <summary>
    /// Represents an archiving rule for call by media type.
    /// </summary>
    public class MediaTypeRule : IArchivingRule
    {
        private readonly ILogger<MediaTypeRule> _logger;
        private readonly MediaTypeArchivingRule _mediaTypeArchivingRule;
        private List<MediaType> _mediaTypes = new List<MediaType>();
        public int Priority => 2;
        public bool StopOnFailure { get; set; }

        public MediaTypeRule(ILogger<MediaTypeRule>  logger,
            MediaTypeArchivingRule mediaTypeArchivingRule)
        {
            _logger = logger;
            _mediaTypeArchivingRule = mediaTypeArchivingRule;
            _mediaTypes = GetMediaTypes();
        }
        /// <summary>
        ///  Applies rule for call recordings by medeia type.
        /// </summary>
        /// <param name="recordings"></param>
        /// <returns></returns>
        public HashSet<int> ApplyRule(CallEvent callEvent)
        {
            return new HashSet<int>(
                callEvent.Recordings
                    .Where(recording => _mediaTypes.Any(mediaType => recording.MediaType == mediaType))
                    .Select(recording => recording.RecordingId));
        }
        /// <summary>
        /// Gets media types from configuration.
        /// </summary>
        /// <returns></returns>
        private List<MediaType> GetMediaTypes()
        {
            foreach (var mediaType in _mediaTypeArchivingRule.MediaTypes)
            {
                _mediaTypes.Add(mediaType);
            }
            return _mediaTypes;
        }
    }
}
