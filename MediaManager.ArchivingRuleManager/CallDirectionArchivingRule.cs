using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaManager.ArchivingRuleManager
{
    /// <summary>
    /// Represents an archiving rule for call by call direction.
    /// </summary>
    public class CallDirectionArchivingRule : IArchivingRule
    {
        private readonly ILogger<CallDirectionArchivingRule> _logger;
        private  CallDirectionRuleConfig _callDirectionRuleConfig;
        private List<CallDirection> _callDirections = new List<CallDirection>();
        public int Priority { get; set; }
        public bool StopOnFailure { get; set; }

        public CallDirectionArchivingRule(ILogger<CallDirectionArchivingRule> logger,
             IOptionsMonitor<CallDirectionRuleConfig> callDirectionRuleConfig)
        {
            _logger = logger;

            _callDirectionRuleConfig = callDirectionRuleConfig.CurrentValue;
            Priority = _callDirectionRuleConfig.Priority;
            StopOnFailure = _callDirectionRuleConfig.StopOnFailure;

            _logger.LogInformation($"CallDirectionArchivingRule initialized with " +
                $"Priority: {Priority}, StopOnFailure: {StopOnFailure}");

            _callDirections = GetCallDirections();

            callDirectionRuleConfig.OnChange(settings =>
            {

                _callDirectionRuleConfig = settings;

                _callDirections.Clear();
                _callDirections = GetCallDirections();

                _logger.LogInformation("CallDirectionRuleConfig changed. Refreshed CallDirections.");
            });
        }
        /// <summary>
        ///  Applies rule for call recordings by call direction.
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        public HashSet<int> ApplyRule(CallEvent callEvent)
        {
            _logger.LogInformation("Applying rule for call recordings by call direction.");

            var recordings = new HashSet<int>();

            if (_callDirections.Any(callDirection => callEvent.CallDirection == callDirection))
            {
                recordings = new HashSet<int>(callEvent.Recordings.Select(recording => recording.RecordingId));
            }

            return recordings;
        
        }
        /// <summary>
        /// Gets call directions from configuration.
        /// </summary>
        /// <returns></returns>
        private List<CallDirection> GetCallDirections()
        {
            _logger.LogInformation("Getting CallDirections from configuration.");

            var callDirections = new List<CallDirection>(_callDirectionRuleConfig.CallDirections);

            _logger.LogInformation("CallDirections retrieved.");

            return callDirections;
        }
    }
}
