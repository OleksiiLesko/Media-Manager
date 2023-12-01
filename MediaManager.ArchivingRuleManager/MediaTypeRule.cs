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
        public int Priority => 1;

        public MediaTypeRule(ILogger<MediaTypeRule>  logger,
            MediaTypeArchivingRule mediaTypeArchivingRule)
        {
            _logger = logger;
            _mediaTypeArchivingRule = mediaTypeArchivingRule;
            _mediaTypes = GetMediaTypes();
        }
        /// <summary>
        ///  Applies rule for call recordings.
        /// </summary>
        /// <param name="recordings"></param>
        /// <returns></returns>
        public List<int> ApplyRule(List<Recording> recordings)
        {
            return recordings
                 .Where(recording => _mediaTypes.All(mediaType => recording.MediaType == mediaType))
                 .Select(recording => recording.RecordingId)
                 .ToList();
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
