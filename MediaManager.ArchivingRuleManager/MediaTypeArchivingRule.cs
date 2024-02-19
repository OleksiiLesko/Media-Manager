using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaManager.ArchivingRuleManager
{
    /// <summary>
    /// Represents an archiving rule for call by media type.
    /// </summary>
    public class MediaTypeArchivingRule : IArchivingRule
    {
        private readonly ILogger<MediaTypeArchivingRule> _logger;
        private readonly MediaTypeRuleConfig _mediaTypeRuleConfig;
        private List<MediaType> _mediaTypes = new List<MediaType>();
        public int Priority { get; set; }
        public bool StopOnFailure { get; set; }

        public MediaTypeArchivingRule(ILogger<MediaTypeArchivingRule>  logger,
             IOptionsMonitor<MediaTypeRuleConfig> mediaTypeRulConfig)
        {
            _logger = logger;

            _mediaTypeRuleConfig = mediaTypeRulConfig.CurrentValue;
            Priority = _mediaTypeRuleConfig.Priority;
            StopOnFailure = _mediaTypeRuleConfig.StopOnFailure;

            _logger.LogInformation($"MediaTypeArchivingRule initialized with " +
             $"Priority: {Priority}, StopOnFailure: {StopOnFailure}");

            _mediaTypes = GetMediaTypes();
            mediaTypeRulConfig.OnChange(settings =>
            {
                _mediaTypes = GetMediaTypes();
                _logger.LogInformation("MediaTypeRulConfig changed. Refreshed MediaTypes");
            });
        }
        /// <summary>
        ///  Applies rule for call recordings by medeia type.
        /// </summary>
        /// <param name="recordings"></param>
        /// <returns></returns>
        public HashSet<int> ApplyRule(CallEvent callEvent)
        {
            _logger.LogInformation("Applying rule for call recordings by medeia type.");

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
            foreach (var mediaType in _mediaTypeRuleConfig.MediaTypes)
            {
                _mediaTypes.Add(mediaType);
                _logger.LogInformation($"Media type added from config: {mediaType}");
            }
            return _mediaTypes;
        }
    }
}
