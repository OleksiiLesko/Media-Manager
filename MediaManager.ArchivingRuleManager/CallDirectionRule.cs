using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;

namespace MediaManager.ArchivingRuleManager
{
    /// <summary>
    /// Represents an archiving rule for call by call direction.
    /// </summary>
    public class CallDirectionRule : IArchivingRule
    {
        private readonly ILogger<CallDirectionRule> _logger;
        private readonly CallDirectionArchivingRule _callDirectionArchivingRule;
        private List<CallDirection> _callDirections = new List<CallDirection>();
        public int Priority => 1;
        public bool StopOnFailure { get; set; }
        public CallDirectionRule(ILogger<CallDirectionRule> logger, CallDirectionArchivingRule callDirectionArchivingRule)
        {
            _logger = logger;
            _callDirectionArchivingRule = callDirectionArchivingRule;
            _callDirections = GetCallDirections();
        }
        /// <summary>
        ///  Applies rule for call recordings by call direction.
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        public HashSet<int> ApplyRule(CallEvent callEvent)
        {
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
            foreach (var callDirection in _callDirectionArchivingRule.CallDirections)
            {
                _callDirections.Add(callDirection);
                _logger.LogInformation($"Gets call direction {callDirection} from configuration ");

            }
            return _callDirections;
        }
    }
}
