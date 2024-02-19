using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MediaManager.ArchivingRuleManager
{
    /// <summary>
    /// Represents an archiving rule manager service.
    /// </summary>
    public class ArchivingRuleManagerService
    {
        private readonly ILogger<ArchivingRuleManagerService> _logger;
        private readonly IArchivingRuleFactory _ruleFactory;
        private List<IArchivingRule> _enabledRules = new List<IArchivingRule>();

        public ArchivingRuleManagerService(
              ILogger<ArchivingRuleManagerService> logger,
              IArchivingRuleFactory ruleFactory)
        {
            _logger = logger;
            _ruleFactory = ruleFactory;
            _enabledRules = _ruleFactory.CreateAllEnabledArchivingRules();
        }
        /// <summary>
        /// Recieves managed rules call.
        /// </summary>
        /// <param name="receivedCallEvent"></param>
        /// <returns></returns>
        public FilteredCall? ApplyRules(CallEvent receivedCallEvent)
        {
            try
            {
                var filteredRecordingIds = ApplyRulesInOrder(receivedCallEvent);

                if (filteredRecordingIds.Count == 0)
                {
                    _logger.LogError($"Filtered recording ids are null for call: {receivedCallEvent.CallId}");
                    return null;
                }

                var filteredRecordings = FilterRecordingsById(receivedCallEvent, filteredRecordingIds);

                if (filteredRecordings.Count == 0)
                {
                    _logger.LogError($"Filtered recordings are null for call: {receivedCallEvent.CallId}");
                    return null;
                }

                _logger.LogInformation($"Filtered recordings by id for call: {receivedCallEvent.CallId}");
                var ruleManagedCall = CreateFilteredCall(receivedCallEvent, filteredRecordings);

                _logger.LogInformation($"Created rull managed call: {ruleManagedCall.CallId}");
                return ruleManagedCall;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while getting rules managed call: " + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Applies rules in the order listed.
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private HashSet<int> ApplyRulesInOrder(CallEvent callEvent)
        {
            HashSet<int> filteredRecordingIdsByRule = new HashSet<int>();

            foreach (var rule in _enabledRules.OrderBy(r => r.Priority))
            {
                if (callEvent.Recordings.Count > 0)
                {
                    _logger.LogInformation("Applying rules in order listed for call: " + callEvent.CallId);

                    var recordingIds = rule.ApplyRule(callEvent);
                    if (recordingIds.Count == 0 && rule.StopOnFailure)
                    {
                        break;
                    }
                    else
                    {
                        filteredRecordingIdsByRule.UnionWith(recordingIds);
                        continue;
                    }
                }
            }

            return filteredRecordingIdsByRule;
        }
        /// <summary>
        /// Filter recordings by recording id.
        /// </summary>
        /// <param name="receivedCallEvent"></param>
        /// <param name="suitableRecordingsIds"></param>
        /// <returns></returns>
        private List<Recording> FilterRecordingsById(CallEvent receivedCallEvent, HashSet<int> suitableRecordingsIds)
        {
            _logger.LogInformation("Filtering recordings by ID for call: " + receivedCallEvent.CallId);

            var filteredRecordings = receivedCallEvent.Recordings
               .Where(recording => suitableRecordingsIds.Any(id => id == recording.RecordingId))
               .ToList();

            return filteredRecordings;
        }
        /// <summary>
        /// Creates managed rules call.
        /// </summary>
        /// <param name="receivedCallEvent"></param>
        /// <param name="filteredRecordings"></param>
        /// <returns></returns>
        private FilteredCall CreateFilteredCall(CallEvent receivedCallEvent, List<Recording> filteredRecordings)
        {
            _logger.LogInformation("Creating filtered call for: " + receivedCallEvent.CallId);

            var ruleManagedCall = new FilteredCall
            {
                CallId = receivedCallEvent.CallId,
                CallStartTime = receivedCallEvent.CallStartTime,
                CallEndTime = receivedCallEvent.CallEndTime,
                CallDirection = receivedCallEvent.CallDirection,
                ArchivingStatus = receivedCallEvent.ArchivingStatus,
                Recordings = filteredRecordings
            };

            return ruleManagedCall;
        }
    }
}
