using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using RulesSettings = MediaManager.Domain.DTOs.RulesSettings;

namespace MediaManager.ArchivingRuleManager
{
    /// <summary>
    /// Represents an archiving rule manager service.
    /// </summary>
    public class ArchivingRuleManagerService
    {
        private readonly ILogger<ArchivingRuleManagerService> _logger;
        private readonly RulesSettings _rulesSettings;
        private List<IArchivingRule> _enabledRules = new List<IArchivingRule>();

        public ArchivingRuleManagerService(
              ILogger<ArchivingRuleManagerService> logger,
              IOptionsMonitor<RulesSettings> rulesSettings,
              ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _rulesSettings = rulesSettings.CurrentValue;
            _enabledRules = GetEnabledRules(loggerFactory);
            rulesSettings.OnChange(settings =>
            {
                _enabledRules.Clear();
                _enabledRules = GetEnabledRules(loggerFactory);
            });
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
                _logger.LogInformation($"Applied rules in the order listed for call: {receivedCallEvent.CallId}");

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
        /// <summary>
        /// Gets enabled rules.
        /// </summary>
        /// <returns></returns>
        private List<IArchivingRule> GetEnabledRules(ILoggerFactory loggerFactory)
        {
            if (_rulesSettings.MediaTypeArchivingRule.Enabled)
            {
                var mediaTypeRule = new MediaTypeRule(
                    loggerFactory.CreateLogger<MediaTypeRule>(),
                    _rulesSettings.MediaTypeArchivingRule);
                _enabledRules.Add(mediaTypeRule);
            }
            if (_rulesSettings.CallDirectionArchivingRule.Enabled)
            {
                var callDirection = new CallDirectionRule(
                    loggerFactory.CreateLogger<CallDirectionRule>(),
                    _rulesSettings.CallDirectionArchivingRule);
                _enabledRules.Add(callDirection);
            }
            return _enabledRules;
        }
    }
}
